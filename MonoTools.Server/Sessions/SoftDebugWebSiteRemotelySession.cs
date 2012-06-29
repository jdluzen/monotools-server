using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;

namespace MonoTools.Server
{
	public class SoftDebugWebSiteRemotelySession : RunWebSiteRemotelySession
	{
		private int sdb_port;

		public SoftDebugWebSiteRemotelySession (NameValueCollection query) : base (query)
		{
		}

		public override void Start (System.Net.HttpListenerContext context)
		{
			Logger.LogDebug ("Starting sdb web site");

			XmlDocument options = context.GetRequestDataAsXml ();

			int start_port = MonoToolsConfigurationManager.ApplicationPortRangeStart;
			int end_port = MonoToolsConfigurationManager.ApplicationPortRangeEnd;
			host = context.Request.LocalEndPoint.Address;
			
			for (sdb_port = start_port; sdb_port <= end_port; sdb_port++) {
				Logger.LogDebug ("  Trying to start with sdb port: {0}", sdb_port);
				XspSession session = StartXspSession (options);

				if (session == null) {
					Logger.LogDebug ("    sdb on port {0} failed", sdb_port);
					continue;
				}

				Logger.LogDebug ("    sdb port {0} is good, returning to VS", sdb_port);
				is_started = true;
				context.WriteString (ToXml ());
				return;
			}

			Logger.LogDebug ("    Ran out of sdb ports to use: {0}-{1}.", start_port, end_port);

			context.ReturnException (new ApplicationException ("Could not find an available sdb port to use."));
		}

		protected override void AddMonoArguments (List<string> args)
		{
			args.Add (string.Format ("--debugger-agent=transport=dt_socket,address={0}:{1},server=y", host.ToString (), sdb_port));
		}

		// If we're ready to go, return the sdbport for VS
		protected override void SerializeSession (XmlWriter writer)
		{
			if (is_started)
				writer.WriteElementString ("sdbport", sdb_port.ToString ());
		}

		// When debugging, we don't want to wait for the Xsp port in StartXspSession,
		// because it won't show up until sdb resumes the process
		protected override void WaitForXspPort ()
		{
		}
	}
}
