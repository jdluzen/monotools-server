using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using MonoTools.Client;

namespace MonoTools.Server
{
	public abstract class BaseSession
	{
		public string ID { get; private set; }
		public string Name { get; private set; }
		public string RemotePath { get; private set; }
		public SessionType Type { get; private set; }
		public bool IsWeb { get; protected set; }
		public bool HasTerminated { get; set; }

		private Dictionary<string, string> need_files = new Dictionary<string, string> ();
		private List<string> empty_directories = new List<string> ();

		protected Process process;
		protected IProcessAsyncOperation async_process;
		protected XspSession xsp;

		protected BaseSession (NameValueCollection query)
		{
			ID = Guid.NewGuid ().ToString ();
			Name = query["name"];
			RemotePath = query["path"];
			Type = Utilities.ParseSessionType (query["type"]);
			IsWeb = Boolean.Parse (query["web"]);

			if (RemotePath.StartsWith ("[temp]"))
				RemotePath = Path.Combine (Path.GetTempPath (), RemotePath.Substring (6));

			RemotePath = RemotePath.Replace ('\\', Path.DirectorySeparatorChar);
		}

		public virtual void ProcessRequest (HttpListenerContext context)
		{
			string route = context.Request.RawUrl.ToLowerInvariant ();

			if (route.Contains ("?"))
				route = route.Substring (0, route.IndexOf ('?'));

			if (route.EndsWith ("xspinfo")) {
				GetXspInfo (context);
				return;
			}

			if (context.Request.HttpMethod == "GET") {
				context.WriteString (ToXml ());
				return;
			}

			if (route.EndsWith ("filelist"))
				ProcessFileList (context);
			else if (route.EndsWith ("files"))
				ReceivedCompressedFile (context);
			else if (route.EndsWith ("start"))
				Start (context);
			else
				ReceivedFile (context);
		}

		void GetXspInfo (HttpListenerContext context)
		{
			XspResult result;

			if (xsp == null)
				result = new XspResult ("Not a web project!");
			else {
				xsp.WaitHandle.WaitOne ();
				result = xsp.Result ?? new XspResult ("Cannot start xsp: Unknown error");
			}

			using (StringWriter sw = new StringWriter ()) {
				using (XmlWriter xw = new XmlTextWriter (sw)) {
					result.Serialize (xw);
				}

				context.WriteString (sw.ToString ());
			}
		}

		public virtual void KillProcess ()
		{
			try {
				if (process != null)
					process.Kill ();
				else if (async_process != null)
					async_process.Cancel ();
				else
					Logger.LogDebug ("KillProcess failed, process was null.");
			} catch { }
		}

		#region Protected Methods
		protected void SetEnvironmentVariables (ProcessStartInfo psi, XmlDocument options)
		{
			foreach (XmlElement variable in options.SelectNodes ("//options/environment/variable")) {
				psi.EnvironmentVariables[variable.GetAttribute ("name")] = variable.InnerText;
				Logger.LogDebug ("  Setting env var {0} to: {1}", variable.GetAttribute ("name"), variable.InnerText);
			}
		}

		protected void SetEnvironmentVariables (StringDictionary env_vars, XmlDocument options)
		{
			foreach (XmlElement variable in options.SelectNodes ("//options/environment/variable")) {
				env_vars[variable.GetAttribute ("name")] = variable.InnerText;
				Logger.LogDebug ("  Setting env var {0} to: {1}", variable.GetAttribute ("name"), variable.InnerText);
			}
		}

		protected Dictionary<string, string> ParseEnvironmentVariables (XmlDocument options)
		{
			var env_vars = new Dictionary<string, string> ();

			foreach (XmlElement variable in options.SelectNodes ("//options/environment/variable")) {
				env_vars[variable.GetAttribute ("name")] = variable.InnerText;
				Logger.LogDebug ("  Setting env var {0} to: {1}", variable.GetAttribute ("name"), variable.InnerText);
			}

			return env_vars;
		}

		protected void Process_Exited (IAsyncOperation op)
		{
			HasTerminated = true;
			OnProcessExited ();
		}

		protected void Process_Exited (object sender, EventArgs e)
		{
			HasTerminated = true;
			OnProcessExited ();
		}

		protected void OnProcessExited ()
		{
			if (ProcessExited != null)
				ProcessExited (this, EventArgs.Empty);
		}
		#endregion

		#region Request Handlers
		private void ProcessFileList (HttpListenerContext context)
		{
			XmlDocument filelist = context.GetRequestDataAsXml ();
			Logger.LogDebug ("  Read filelist - {0} nodes.", filelist.DocumentElement.ChildNodes.Count);

			// Make sure our output path root exists
			if (!Directory.Exists (RemotePath))
				Directory.CreateDirectory (RemotePath);

			// List all the files that already exist in this path.
			// We will need to delete the ones that are no longer in use
			List<string> existing_files = new List<string> (Directory.GetFiles (RemotePath, "*", SearchOption.AllDirectories));
			Logger.LogDebug ("  {0} files already exist at: {1}", existing_files.Count, RemotePath);

			foreach (XmlElement file in filelist.DocumentElement.ChildNodes) {
				if (file.Name != "file")
					continue;

				string id = file.GetAttribute ("id");
				string filename = Path.Combine (RemotePath, file.InnerText.Replace ('\\', Path.DirectorySeparatorChar));

				// Mark that this file is used in this session
				existing_files.Remove (filename);

				if (File.Exists (filename)) {
					string hash = Utilities.CreateHash (filename);

					if (hash == id) {
						// File hasn't changed
						Logger.LogDebug ("  File is unchanged: {0}", filename);

						continue;
					} else {
						// File has changed, delete the old one
						Logger.LogDebug ("  File has changed: {0}", filename);
						Logger.LogDebug ("    Existing hash: {0}", hash);
						Logger.LogDebug ("    New hash: {0}", id);

						File.Delete (filename);
					}
				}

				need_files.Add (file.InnerText, Guid.NewGuid ().ToString ().Replace ("-", ""));
			}

			// Delete files that aren't used in this session
			foreach (string file in existing_files) {
				Logger.LogDebug ("  Deleting excess file: {0}", file);
				File.Delete (file);
			}

			// Delete directories that aren't used in this session
			DeleteEmptyDirectories (RemotePath);

			// Add back any empty directories the user requested
			foreach (XmlElement dir in filelist.SelectNodes ("//filelist/directory"))
				if (!Directory.Exists (Path.Combine (RemotePath, dir.InnerText.Replace ('\\', Path.DirectorySeparatorChar)))) {
					Logger.LogDebug ("  Creating empty directory: {0}", Path.Combine (RemotePath, dir.InnerText.Replace ('\\', Path.DirectorySeparatorChar)));
					Directory.CreateDirectory (Path.Combine (RemotePath, dir.InnerText.Replace ('\\', Path.DirectorySeparatorChar)));
				}

			context.WriteString (FileListToXml ());
		}

		private void ReceivedCompressedFile (HttpListenerContext context)
		{
			string temp_file = Path.GetTempFileName ();
			Logger.LogDebug ("  Saving compressed file as: {0}", temp_file);

			context.SavePostedFile (temp_file);
			Logger.LogDebug ("  File saved.");

			ZipInputStream zip = new ZipInputStream (File.OpenRead (temp_file));
			ZipEntry ze;

			while ((ze = zip.GetNextEntry ()) != null) {
				string filename = Path.Combine (RemotePath, FindFile (ze.Name));
				Logger.LogDebug ("  Extracting: {0}", filename);

				if (!Directory.Exists (Path.GetDirectoryName (filename)))
					Directory.CreateDirectory (Path.GetDirectoryName (filename));

				byte[] buffer = new byte[2048];
				int len;

				// Don't use using() because we don't want to dispose it
				// or it will close our stream
				BinaryReader sr = new BinaryReader (zip);

				using (BinaryWriter sw = new BinaryWriter (new FileStream (filename, FileMode.Create)))
					while ((len = sr.Read (buffer, 0, buffer.Length)) > 0)
						sw.Write (buffer, 0, len);
			}

			Logger.LogDebug ("  Extracting completed successfully.");
			File.Delete (temp_file);

			context.ReturnSuccess ();
		}

		private void ReceivedFile (HttpListenerContext context)
		{
			string route = context.Request.RawUrl;
			string file_id = route.Substring (51);
			string filename = Path.Combine (RemotePath, FindFile (file_id));

			context.SavePostedFile (filename);
			context.ReturnSuccess ();
		}

		public abstract void Start (HttpListenerContext context);
		#endregion

		#region Private Methods
		private string FindFile (string id)
		{
			foreach (KeyValuePair<string, string> file in need_files)
				if (file.Value == id)
					return file.Key.Replace ('\\', Path.DirectorySeparatorChar);

			return null;
		}

		private string FileListToXml ()
		{
			// Create file list
			using (StringWriter sw = new StringWriter ()) {
				using (XmlTextWriter xw = new XmlTextWriter (sw)) {
					xw.WriteStartElement ("filelist");

					foreach (KeyValuePair<string, string> file in need_files) {
						xw.WriteStartElement ("file");
						xw.WriteAttributeString ("id", file.Value);
						xw.WriteString (file.Key);
						xw.WriteEndElement ();
					}

					xw.WriteEndElement ();
				}

				Logger.LogDebug ("  Need {0} files from client.", need_files.Count);
				return sw.ToString ();
			}
		}

		private void DeleteEmptyDirectories (string path)
		{
			foreach (string dir in Directory.GetDirectories (path))
				DeleteEmptyDirectories (dir);

			if (Directory.GetFiles (path).Length == 0 && Directory.GetDirectories (path).Length == 0) {
				Logger.LogDebug ("  Deleting empty directory: {0}", path);
				Directory.Delete (path);
			}
		}
		#endregion

		#region Serialization
		public string ToXml ()
		{
			using (StringWriter sw = new StringWriter ()) {
				using (XmlWriter xw = new XmlTextWriter (sw)) {
					xw.WriteStartElement ("session");
					xw.WriteElementString ("id", ID);
					xw.WriteElementString ("name", Name);
					xw.WriteElementString ("path", RemotePath);
					xw.WriteElementString ("type", Utilities.OutputSessionType (Type));
					xw.WriteElementString ("web", IsWeb.ToString ().ToLowerInvariant ());
					xw.WriteElementString ("hasterminated", HasTerminated.ToString ().ToLowerInvariant ());

					// Give derived classes a chance to add stuff
					SerializeSession (xw);

					xw.WriteEndElement ();
				}

				return sw.ToString ();
			}
		}

		protected virtual void SerializeSession (XmlWriter writer)
		{
		}
		#endregion

		#region Events
		public event EventHandler ProcessExited;
		#endregion
	}
}
