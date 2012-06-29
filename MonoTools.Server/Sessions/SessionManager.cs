using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Collections.Specialized;
using MonoTools.Client;

namespace MonoTools.Server
{
	public class SessionManager
	{
		Dictionary<string, BaseSession> sessions = new Dictionary<string, BaseSession> ();
		Dictionary<int, bool> sdb_ports = new Dictionary<int, bool> ();

		public SessionManager ()
		{
			for (int port = Session.PortRangeStart; port <= Session.PortRangeEnd; port++)
				sdb_ports[port] = false;
		}

		public void CreateSession (HttpListenerContext context)
		{
			NameValueCollection query = context.Request.QueryString;

			string session_type = query["type"];
			bool is_web = false;

			if (string.IsNullOrEmpty (session_type))
				throw new ArgumentNullException ("Session type was not specified.");

			bool.TryParse (query["web"], out is_web);

			SessionType type = Utilities.ParseSessionType (session_type);

			BaseSession s;

			// Create the correct session
			switch (type) {
			case SessionType.Run:
				if (is_web)
					s = new RunWebSiteRemotelySession (context.Request.QueryString);
				else
					s = new RunExecutableRemotelySession (context.Request.QueryString);
				break;
			case SessionType.RunDebugger: {
				int port = AllocateSdbPort ();

				if (is_web)
					s = new SoftDebugWebSiteRemotelySession (context.Request.QueryString);
				else
					s = new SoftDebugExecutableRemotelySession (context.Request.QueryString);
				s.ProcessExited += delegate {
					FreeSdbPort (port);
				};
				break;
			}
			case SessionType.RunPackager:
				s = new CreateRpmSession (context.Request.QueryString);
				break;
			default:
				throw new NotImplementedException (string.Format ("Session type {0} not implemented yet.", type));
			}
			
			// Store the session
			s.ProcessExited += new EventHandler (Session_ProcessExited);
			sessions.Add (s.ID, s);

			// Return the session
			context.WriteString (s.ToXml ());
		}

		private int AllocateSdbPort ()
		{
			foreach (var port in sdb_ports.Keys) {
				if (sdb_ports[port])
					continue;
				sdb_ports[port] = true;
				return port;
			}

			return -1;
		}

		private void FreeSdbPort (int port)
		{
			sdb_ports[port] = false;
		}

		private void Session_ProcessExited (object sender, EventArgs e)
		{
			BaseSession s = (BaseSession)sender;

			Logger.LogInfo ("Session exited: {0}-{1}", s.Name, s.ID);
			sessions.Remove (s.ID);
		}

		public void RouteSessionRequest (HttpListenerContext context)
		{
			string route = context.Request.RawUrl.ToLowerInvariant ();

			if (route.Length < 45) {
				Logger.LogWarning ("Uri not long enough to contains session id");
				throw new ArgumentNullException ("Uri does not contain session id.");
			}

			string session_id = route.Substring (9, 36);

			if (!sessions.ContainsKey (session_id)) {
				Logger.LogWarning ("Session '{0}' cannot be found.", session_id);
				context.ReturnException (new ArgumentException (string.Format ("Session '{0}' cannot be found.", session_id)));
				return;
			}

			BaseSession session = sessions[session_id];

			if (context.Request.HttpMethod == "DELETE") {
				session.KillProcess ();
				sessions.Remove (session_id);
				context.ReturnSuccess ();
				return;
			}

			session.ProcessRequest (context);
		}
	}
}
