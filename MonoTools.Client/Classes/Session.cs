using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;

namespace MonoTools.Client
{
	//<session>
	//  <id>aabb4eg5477</id>
	//  <name>BlogEngine.Net</name>
	//  <path>/tmp/monovs-blogengine.net</path>
	//  <type>run</type>
	//  <web>true</web>
	//</session>
	public class Session
	{
		public string ID { get; private set; }
		public string Name { get; private set; }
		public string RemotePath { get; private set; }
		public bool IsWeb { get; private set; }
		public bool HasTerminated { get; private set; }
		public SessionType Type { get; private set; }
		
		public bool UseCompression { get; set; }
		
		public string BaseDirectory { get; set; }
		public string StartupExe { get; set; }
		public string UserArguments { get; set; }
		public string MonoArguments { get; set; }
		public Dictionary<string, string> EnvironmentVariables { get; private set; }
		public Dictionary<string, string> StartOptions { get; private set; }
		
		private Dictionary<string, string> Files = new Dictionary<string,string> ();
		private List<string> EmptyDirectories = new List<string> ();

		public bool NeedsTerminal { get; set; }

		public XspOptions XspOptions { get; set; }

		private WebClient running_request;
		private bool cancel_set;
		
		internal Session (XmlElement response)
		{
			ID = response["id"].InnerText;
			Name = response["name"].InnerText;
			RemotePath = response["path"].InnerText;
			IsWeb = bool.Parse (response["web"].InnerText);
			HasTerminated = bool.Parse (response["hasterminated"].InnerText);
			Type = ParseSessionType (response["type"].InnerText);
			
			UseCompression = true;
			
			UserArguments = string.Empty;
			MonoArguments = string.Empty;
			
			EnvironmentVariables = new Dictionary<string,string> ();
			StartOptions = new Dictionary<string,string> ();
		}

		#region Public Methods
		public void AddFile (string file)
		{
			string hash = CreateHash (file);

			Files.Add (GetRelativePath (file), hash);			
		}
		
		public void AddEmptyDirectory (string directory)
		{
			EmptyDirectories.Add (GetRelativePath (directory));
		}
		
		public void CancelAsync ()
		{
			if (running_request != null)
				running_request.CancelAsync ();
		}
		
		public void SynchronizeFiles ()
		{
			string filelist = CreateFileList ();
			
			// Send list, get back of list of files to send
			XmlDocument needfiles = Connection.HttpPost (string.Format ("/session/{0}/filelist", ID), filelist);

			// Short circuit if the server doesn't need any files
			if (needfiles.DocumentElement.ChildNodes.Count == 0)
				return;
				
			if (UseCompression) {
				CompressFiles (needfiles);
				return;
			}
			
			// Send each file
			foreach (XmlElement xe in needfiles.SelectNodes ("//filelist/file")) {
				string url = string.Format ("/session/{0}/file/{1}", ID, xe.GetAttribute ("id"));
				string file = Path.Combine (BaseDirectory, xe.InnerText);

				Connection.HttpPostFile (url, file);
			}
		}

		public void SynchronizeFilesAsync ()
		{
			if (UseCompression == false)
				throw new ArgumentException ("Asynchronous synchronizing currently only works with UseCompression = true.");

			cancel_set = false;
			string filelist = CreateFileList ();
			
			OnSynchronizeStepStarting (SynchonizeStep.CompareWithServer, 0, 0, TimeSpan.Zero);
			
			if (cancel_set)
				return;
				
			// Upload the file list to the server
			WebClient wc = new WebClient ();
			string url = string.Format ("/session/{0}/filelist", ID);

			byte[] buffer = Encoding.ASCII.GetBytes (filelist);
			
			running_request = wc;
			wc.UploadDataCompleted += new UploadDataCompletedEventHandler (FileList_UploadDataCompleted);
			wc.UploadDataAsync (Connection.CreateUri (url), buffer);
		}

		public XmlDocument Start ()
		{
			if (!IsWeb && string.IsNullOrEmpty (StartupExe))
				throw new ApplicationException ("Non-web projects must have a startup exe");

			string url = string.Format ("/session/{0}/start", ID);
			string data;

			using (StringWriter sw = new StringWriter ()) {
				using (XmlTextWriter xw = new XmlTextWriter (sw)) {
					xw.WriteStartElement ("options");
					xw.WriteElementString ("executable", StartupExe);
					xw.WriteElementString ("arguments", UserArguments);
					xw.WriteElementString ("monoarguments", MonoArguments);
					xw.WriteElementString ("needsterminal", NeedsTerminal.ToString ());

					if (EnvironmentVariables.Count > 0) {
						xw.WriteStartElement ("environment");

						foreach (var item in EnvironmentVariables) {
							xw.WriteStartElement ("variable");
							xw.WriteAttributeString ("name", item.Key);
							xw.WriteString (item.Value);
							xw.WriteEndElement ();
						}

						xw.WriteEndElement ();
					}

					foreach (var opt in StartOptions)
						xw.WriteElementString (opt.Key, opt.Value);

					if (XspOptions != null)
						XspOptions.Serialize (xw);

					xw.WriteEndElement ();
				}

				data = sw.ToString ();
			}

			return Connection.HttpPost (url, data);
		}
		
		public void Kill ()
		{
			string url = string.Format ("/session/{0}", ID);
			Connection.HttpDelete (url, string.Empty);
		}
		
		public void DownloadPackage (string outputPath)
		{
			WebClient wc = new WebClient ();
			string url = string.Format ("/session/{0}/package", ID);

			wc.DownloadFile (Connection.CreateUri (url), outputPath);
		}

		public void BeginReadOutput ()
		{
			WebClient wc = new WebClient ();
			string url = string.Format ("/session/{0}/output", ID);

			var req = HttpWebRequest.Create (Connection.CreateUri (url));
			req.Method = "GET";

			req.BeginGetResponse (delegate (IAsyncResult result) {
				var res = req.EndGetResponse (result);
				var stream = res.GetResponseStream ();
				while (true) {
					byte[] header = new byte [3];
					stream.Read (header, 0, 3);

					int size = BitConverter.ToInt16 (header, 1);
					byte[] data = new byte[size];
					stream.Read (data, 0, size);

					if (header[0] == 0x01) {
						string line = Encoding.UTF8.GetString (data);
						OnOutputReceived (line, false);
					} else if (header[0] == 0x02) {
						string line = Encoding.UTF8.GetString (data);
						OnOutputReceived (line, true);
					} else if (header[0] == 0x00) {
						int exitcode = BitConverter.ToInt32 (data, 0);
						Console.WriteLine ("PROCESS EXITED: {0}", exitcode);
						break;
					}
				}
			}, null);
		}

		public void GetXspInfoAsync ()
		{
			WebClient wc = new WebClient ();
			string url = string.Format ("/session/{0}/xspinfo", ID);

			var req = HttpWebRequest.Create (Connection.CreateUri (url));
			req.Method = "GET";

			req.BeginGetResponse (delegate (IAsyncResult result) {
				var res = req.EndGetResponse (result);
				XmlDocument doc = new XmlDocument ();
				doc.Load (res.GetResponseStream ());
				XmlNode node = doc.SelectSingleNode ("/xsp-result");
				var xsp_result = new XspResult (node);
				OnWebServerStarted (xsp_result);
			}, null);
		}

		public XspResult GetXspInfo ()
		{
			WebClient wc = new WebClient ();
			string url = string.Format ("/session/{0}/xspinfo", ID);

			var req = HttpWebRequest.Create (Connection.CreateUri (url));
			req.Method = "GET";

			var res = req.GetResponse ();
			XmlDocument doc = new XmlDocument ();
			doc.Load (res.GetResponseStream ());
			XmlNode node = doc.SelectSingleNode ("/xsp-result");

			return new XspResult (node);
		}

		public void SetEnvironmentVariables (string env)
		{
			EnvironmentVariables.Clear ();
			
			if (string.IsNullOrEmpty (env))
				return;
				
			string[] pairs = env.Split (';');
			
			foreach (var p in pairs) {
				string[] v = p.Split ('=');
				
				EnvironmentVariables[v[0]] = v[1];
			}
		}
		#endregion

		#region SynchronizeFilesAsync Implementation
		private string zip_file;
		private DateTime start;
		
		private void FileList_UploadDataCompleted (object sender, UploadDataCompletedEventArgs e)
		{
			running_request = null;

			if (cancel_set)
				return;
			
			// Check for cancelled or error
			if (e.Cancelled || e.Error != null) {
				OnFileSynchronizeError (e.Cancelled, e.Error, e.UserState);
				return;
			}
			
			// Read the list of files the server needs to be sent
			XmlDocument needfiles = new XmlDocument ();
			needfiles.LoadXml (Encoding.UTF8.GetString (e.Result));

			OnSynchronizeStepCompleted (SynchonizeStep.CompareWithServer, 0, 0, DateTime.Now.Subtract (start));
			
			// Short circuit if the server doesn't need any files
			if (needfiles.DocumentElement.ChildNodes.Count == 0) {
				OnSynchronizeCompleted ();
				return;
			}

			CompressFilesAsync (needfiles);
		}

		private void CompressFilesAsync (XmlDocument needfiles)
		{
			if (cancel_set)
				return;

			XmlNodeList filelist = needfiles.SelectNodes ("//filelist/file");

			int total_files = filelist.Count;
			
			OnSynchronizeStepStarting (SynchonizeStep.CompressFiles, 0, total_files, TimeSpan.Zero);		
			start = DateTime.Now;
			
			// Create the zip file
			zip_file = Path.GetTempFileName ();

			ZipFile zip = ZipFile.Create (zip_file);
			zip.BeginUpdate ();

			foreach (XmlElement xe in filelist) {
				string file = Path.Combine (BaseDirectory, xe.InnerText);

				zip.Add (file, xe.GetAttribute ("id"));
			}

			zip.CommitUpdate ();
			zip.Close ();

			if (cancel_set)
				return;

			OnSynchronizeStepCompleted (SynchonizeStep.CompressFiles, total_files, total_files, DateTime.Now.Subtract (start));

			// Send compressed file
			OnSynchronizeStepStarting (SynchonizeStep.SendFiles, 0, 0, TimeSpan.Zero);
			start = DateTime.Now;

			string url = string.Format ("/session/{0}/files", ID);
			
			WebClient wc = new WebClient ();

			running_request = wc;
			wc.UploadFileCompleted += new UploadFileCompletedEventHandler (ZipFile_UploadFileCompleted);
			wc.UploadProgressChanged += new UploadProgressChangedEventHandler (ZipFile_UploadProgressChanged);
			wc.UploadFileAsync (Connection.CreateUri (url), zip_file);
		}

		private void ZipFile_UploadProgressChanged (object sender, UploadProgressChangedEventArgs e)
		{
			OnSynchronizeStepProgress (SynchonizeStep.SendFiles, e.BytesSent, e.TotalBytesToSend, DateTime.Now.Subtract (start));
		}

		private void ZipFile_UploadFileCompleted (object sender, UploadFileCompletedEventArgs e)
		{
			running_request = null;
			
			// Delete our temporary zip file
			try {
				File.Delete (zip_file);
			} catch { }
			
			// Check for cancelled or error
			if (e.Cancelled || e.Error != null) {
				OnFileSynchronizeError (e.Cancelled, e.Error, e.UserState);
				return;
			}
			
			// Notify user we are done
			OnSynchronizeStepCompleted (SynchonizeStep.SendFiles, 0, 0, DateTime.Now.Subtract (start));
			OnSynchronizeCompleted ();
		}
		#endregion

		#region Private Methods
		private string CreateFileList ()
		{
			string filelist;

			// Create file list
			using (StringWriter sw = new StringWriter ()) {
				using (XmlTextWriter xw = new XmlTextWriter (sw)) {
					xw.WriteStartElement ("filelist");

					foreach (KeyValuePair<string, string> file in Files) {
						xw.WriteStartElement ("file");
						xw.WriteAttributeString ("id", file.Value);
						xw.WriteString (file.Key);
						xw.WriteEndElement ();
					}

					foreach (string dir in EmptyDirectories)
						xw.WriteElementString ("directory", dir);
									
					xw.WriteEndElement ();
				}

				filelist = sw.ToString ();
			}

			return filelist;
		}
		
		private static string CreateHash (string filename)
		{
			using (HashAlgorithm hashAlg = SHA1.Create ())
				using (Stream file = new FileStream (filename, FileMode.Open, FileAccess.Read))
					return BitConverter.ToString (hashAlg.ComputeHash (file)).Replace ("-", "");
		}

		private string GetRelativePath (string filename)
		{
			string base_directory = BaseDirectory.TrimEnd ('\\'); ;
			return filename.Substring (base_directory.Length + 1);
		}

		private void CompressFiles (XmlDocument needfiles)
		{
			DateTime start = DateTime.Now;

			string zip_file = Path.GetTempFileName ();

			ZipFile zip = ZipFile.Create (zip_file);
			zip.BeginUpdate ();

			foreach (XmlElement xe in needfiles.SelectNodes ("//filelist/file")) {
				string file = Path.Combine (BaseDirectory, xe.InnerText);
				zip.Add (file, xe.GetAttribute ("id"));
			}

			zip.CommitUpdate ();
			zip.Close ();

			Console.WriteLine ("zip: " + DateTime.Now.Subtract (start).ToString ());

			start = DateTime.Now;

			string url = string.Format ("/session/{0}/files", ID);
			Connection.HttpPostFile (url, zip_file);
			File.Delete (zip_file);
			Console.WriteLine ("zip send: " + DateTime.Now.Subtract (start).ToString ());
		}

		private SessionType ParseSessionType (string type)
		{
			switch (type.ToLowerInvariant ()) {
				case "run": return SessionType.Run;
				case "debug": return SessionType.RunDebugger;
				case "package": return SessionType.RunPackager;
			}

			throw new ArgumentOutOfRangeException (string.Format ("Unsupported SessionType: {0}", type));
		}
		#endregion
		
		#region Event Raisers
		protected void OnSynchronizeStepStarting (SynchonizeStep step, long complete, long total, TimeSpan elapsedTime)
		{
			if (SynchronizeStepStarting != null)
				SynchronizeStepStarting (this, new SynchronizeFilesEventArgs (step, complete, total, elapsedTime));
		}

		protected void OnSynchronizeStepCompleted (SynchonizeStep step, long complete, long total, TimeSpan elapsedTime)
		{
			if (SynchronizeStepCompleted != null)
				SynchronizeStepCompleted (this, new SynchronizeFilesEventArgs (step, complete, total, elapsedTime));
		}

		protected void OnSynchronizeStepProgress (SynchonizeStep step, long complete, long total, TimeSpan elapsedTime)
		{
			if (SynchronizeStepProgress != null)
				SynchronizeStepProgress (this, new SynchronizeFilesEventArgs (step, complete, total, elapsedTime));
		}

		protected void OnSynchronizeCompleted ()
		{
			if (SynchronizeCompleted != null)
				SynchronizeCompleted (this, EventArgs.Empty);
		}
		
		protected void OnFileSynchronizeError (bool cancelled, Exception error, object state)
		{
			if (FileSynchronizeError != null)
				FileSynchronizeError (this, new ServerResponseEventArgs (cancelled, error, state));
		}

		protected void OnOutputReceived (string output, bool is_error)
		{
			if (OutputReceived != null)
				OutputReceived (this, new OutputReceivedEventArgs (output, is_error));
		}

		protected void OnWebServerStarted (XspResult result)
		{
			if (WebServerStarted != null)
				WebServerStarted (this, new WebServerStartedEventArgs (result));
		}
		#endregion

		#region Public Events
		public event EventHandler<SynchronizeFilesEventArgs> SynchronizeStepStarting;
		public event EventHandler<SynchronizeFilesEventArgs> SynchronizeStepCompleted;
		public event EventHandler<SynchronizeFilesEventArgs> SynchronizeStepProgress;
		public event EventHandler<ServerResponseEventArgs> FileSynchronizeError;
		public event EventHandler SynchronizeCompleted;
		public event EventHandler<OutputReceivedEventArgs> OutputReceived;
		public event EventHandler<WebServerStartedEventArgs> WebServerStarted;
		#endregion

		#region Default Port Range
		public const int PortRangeStart = 8806;
		public const int PortRangeEnd = 8850;
		public const int WebPortRangeStart = 8851;
		public const int WebPortRangeEnd = 8872;
		#endregion
	}
}
