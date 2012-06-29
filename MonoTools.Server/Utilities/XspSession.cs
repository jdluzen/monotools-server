using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using MonoTools.Client;

namespace MonoTools.Server
{
	public class XspSession
	{
		XspOptions options;
		ProcessStartInfo psi;
		Process process;

		ManualResetEvent ready_event = new ManualResetEvent (false);
		
		public event EventHandler<WebServerStartedEventArgs> WebServerStartedEvent;

		public WaitHandle WaitHandle {
			get { return ready_event; }
		}

		public XspResult Result {
			get; private set;
		}

		public Process Process {
			get { return process; }
		}

		public bool SdbPortInUse { get; private set; }

		void OnWebServerStartedEvent (XspResult result)
		{
			if (WebServerStartedEvent != null)
				WebServerStartedEvent (this, new WebServerStartedEventArgs (result));
		}

		public XspSession (XspOptions options, string remote_path, List<string> mono_args, StringDictionary env_vars)
		{
			this.options = options;

			var mono = Utilities.GetMonoExecutable ();

			var current = Assembly.GetCallingAssembly ();
			var current_dir = Path.GetDirectoryName (current.Location);

			Console.WriteLine (current_dir);

			var webserver = Path.Combine (current_dir, "MonoTools.WebServer.exe");

			List<string> args = new List<string> ();
			args.Add ("--debug");
			args.AddRange (mono_args);
			args.Add (string.Format ("\"{0}\"", webserver));

			var process_args = string.Join (" ", args.ToArray ());
			psi = new ProcessStartInfo (mono, process_args);
			psi.UseShellExecute = false;
			psi.RedirectStandardInput = true;
			psi.RedirectStandardOutput = true;
		}
		
		public bool Start ()
		{
			process = Process.Start (psi);

			bool started = false;

			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
				Console.WriteLine ("OUTPUT: {0}", e.Data ?? "null");

				if (started)
					return;


				//if (string.IsNullOrEmpty (e.Data))
				//	return;

				started = true;
				process.CancelOutputRead ();

				// SDB port in use
				if (e.Data == null) {
					Logger.LogDebug ("e.Data is null");
					SdbPortInUse = true;
					ready_event.Set ();
					Logger.LogDebug ("ready_event set");
					return;
				}

				XmlDocument doc = new XmlDocument ();
				doc.LoadXml (e.Data);

				Logger.LogDebug ("loaded e.Data doc");
				var node = doc.SelectSingleNode ("/xsp-result");
				Result = new XspResult (node);
				ready_event.Set ();
				OnWebServerStartedEvent (Result);
			};

			// See if sdb port is in use before going on
			if (process.WaitForExit (1000))
				return false;

			// The process may have already exited due to sdb port in use
			try {
				Logger.LogDebug ("sending xsp: {0}", options.ToXml ());
				process.StandardInput.WriteLine (options.ToXml ());
				process.StandardInput.Close ();
			} catch {
				Logger.LogDebug ("Couldn't write to xsp standardinput");
			}
			
			Logger.LogDebug ("going to begin output readline");
			process.BeginOutputReadLine ();
			Logger.LogDebug ("beginoutreadline finished");

			return true;
		}

	}
}
