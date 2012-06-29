using System;
using System.Net;

namespace MonoTools.Server
{
	public class MonoToolsServer
	{
		private HttpListener http_server;

		public int Port { get; set; }
		public SessionManager Sessions { get; private set; }

		public MonoToolsServer ()
		{
			Sessions = new SessionManager ();
		}

		public void SetLogLevel (EnabledLoggingLevel level)
		{
			Logger.EnabledLevel = level;
		}

		#region HttpListener
		public void Start ()
		{
			if (Port == 0)
				throw new ArgumentException ("Port must be specified prior to calling Start ().");

			if (http_server == null) {
				http_server = new HttpListener ();
				http_server.Prefixes.Add (string.Format ("http://*:{0}/", Port));
			}

			http_server.Start ();
			http_server.BeginGetContext (ListenerCallback, null);
		}

		private void ListenerCallback (IAsyncResult result)
		{
			HttpListenerContext context = null;

			try {
				Logger.LogDebug ("ListenerCallback: ENTER");

				// Call EndGetContext to complete the asynchronous operation.
				context = http_server.EndGetContext (result);

				Logger.LogDebug ("  ListenerCallback: Got request.");

				// Listen for the next request
				http_server.BeginGetContext (ListenerCallback, null);

				RouteRequest (context);
			} catch (Exception ex) {
				Logger.LogError ("  ListenerCallback Exception: {0}", ex);

				if (context != null)
					context.ReturnException (ex);
			} finally {
				Logger.LogDebug ("ListenerCallback: LEAVE");
			}
		}

		private void RouteRequest (HttpListenerContext context)
		{
			Logger.LogDebug ("RouteRequest: ENTER");

			HttpListenerRequest request = context.Request;

			string route = request.RawUrl.ToLowerInvariant ();

			if (route.Contains ("?"))
				route = route.Substring (0, route.IndexOf ('?'));

			Logger.LogDebug ("  Routing: {0} - {1}", request.HttpMethod, route);

			if (request.RawUrl.Contains ("?"))
				Logger.LogDebug ("  QueryString: {0}", request.RawUrl.Substring (request.RawUrl.IndexOf ('?')).TrimStart ('?'));
				

			if (request.HttpMethod == "GET") {

				if (route == "/server/details")
					GetServerDetails (context);
				else if (route.StartsWith ("/session/"))
					Sessions.RouteSessionRequest (context);
				else
					Logger.LogWarning ("  ^^ COULD NOT ROUTE PREVIOUS REQUEST");

			} else if (request.HttpMethod == "POST" || request.HttpMethod == "DELETE") {

				if (route == "/sessions")
					Sessions.CreateSession (context);
				else if (route.StartsWith ("/session/"))
					Sessions.RouteSessionRequest (context);
				else
					Logger.LogWarning ("  ^^ COULD NOT ROUTE PREVIOUS REQUEST");
			}

			Logger.LogDebug ("RouteRequest: LEAVE");
		}
		#endregion

		#region Server Request Handlers
		public void GetServerDetails (HttpListenerContext context)
		{
			string details = "<server><version>1.0</version><os>Linux</os><secureonly>false</secureonly><supportsrun>true</supportsrun><supportssdb>true</supportssdb><supportsmdb>true</supportsmdb><supportsrpm>true</supportsrpm></server>";

			context.WriteString (details);
		}
		#endregion
	}
}
