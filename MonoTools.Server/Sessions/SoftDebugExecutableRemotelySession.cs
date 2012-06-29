using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using MonoTools.Client;

namespace MonoTools.Server
{
	public class SoftDebugExecutableRemotelySession : RunExecutableRemotelySession
	{
		private bool is_started;
		private int sdb_port;
		string host;

		public SoftDebugExecutableRemotelySession (NameValueCollection query) : base (query)
		{
		}

		public override void Start (HttpListenerContext context)
		{
			Logger.LogInfo ("Starting sdb executable");
			XmlDocument options = context.GetRequestDataAsXml ();

			int start_port = MonoToolsConfigurationManager.ApplicationPortRangeStart;
			int end_port = MonoToolsConfigurationManager.ApplicationPortRangeEnd;
			host = context.Request.LocalEndPoint.Address.ToString ();

			for (sdb_port = start_port; sdb_port <= end_port; sdb_port++) {
				Logger.LogDebug ("  Trying to start with sdb port: {0}", sdb_port);

				var p = StartProcess (options);

				// Wait a second and see if the process exits
				if (!p.WaitForCompleted (1000)) {
					is_started = true;
					Logger.LogDebug ("    sdb port {0} is good, returning to VS", sdb_port);
					context.WriteString (ToXml ());

					return;
				}

				Logger.LogDebug ("    sdb on port {0} failed", sdb_port);
			}

			Logger.LogDebug ("    Ran out of sdb ports to use: {0}-{1}.", start_port, end_port);

			context.ReturnException (new ApplicationException ("Could not find an available sdb port to use."));
		}

		protected override void AddMonoArguments (List<string> args)
		{
			args.Add (string.Format ("--debugger-agent=transport=dt_socket,address={0}:{1},server=y", host, sdb_port));
		}

		// If we're ready to go, return the sdbport for VS
		protected override void SerializeSession (XmlWriter writer)
		{
			if (is_started)
				writer.WriteElementString ("sdbport", sdb_port.ToString ());
		}

		// We check for success in acquiring the sdb port by the terminal
		// not immediately exiting.  If it always pauses, we don't know if
		// the sdb session launch was successful.
		protected override bool PauseTerminal { get { return false; } }
	}
}
