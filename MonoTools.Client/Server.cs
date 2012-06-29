using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Web;
using System.Threading;

namespace MonoTools.Client
{
	public class Server
	{
		private WebClient wc;
		private Thread t;
		
		public Server (string ip, int port)
		{
			Connection.SetServer (ip, port);

			wc = new WebClient ();

			wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler (GetServerDetails_DownloadStringCompleted);
		}
		
		#region GetServerDetails
		public ServerDetails GetServerDetails ()
		{
			XmlDocument response = Connection.HttpGet ("/server/details");

			return new ServerDetails (response.DocumentElement);
		}

		public void GetServerDetailsAsync (object state)
		{
			wc.DownloadStringAsync (Connection.CreateUri ("/server/details"), state);
		}

		private void GetServerDetails_DownloadStringCompleted (object sender, DownloadStringCompletedEventArgs e)
		{
			if (GetServerDetailsCompleted != null) {
				if (e.Cancelled || e.Error != null)
					GetServerDetailsCompleted (this, new ServerResponseEventArgs (e.Cancelled, e.Error, e.UserState));
				else	
					GetServerDetailsCompleted (this, new ServerResponseEventArgs (e.Result, e.UserState));
				
			}
		}
		
		public event EventHandler<ServerResponseEventArgs> GetServerDetailsCompleted;
		#endregion

		#region CreateSession
		public Session CreateSession (string name, string remotePath, SessionType type, bool isWeb)
		{
			string url = "/sessions?name={0}&path={1}&type={2}&web={3}";
			url = string.Format (url, HttpUtility.UrlEncode (name), HttpUtility.UrlEncode (remotePath), OutputSessionType (type), HttpUtility.UrlEncode (isWeb.ToString ().ToLowerInvariant ()));

			XmlDocument response = Connection.HttpPost (url, null, null);
	
			return new Session (response.DocumentElement);
		}

		
		public void CreateSessionAsync (string name, string remotePath, SessionType type, bool isWeb, object state2)
		{
			string url = "/sessions?name={0}&path={1}&type={2}&web={3}";
			url = string.Format (url, HttpUtility.UrlEncode (name), HttpUtility.UrlEncode (remotePath), OutputSessionType (type), HttpUtility.UrlEncode (isWeb.ToString ().ToLowerInvariant ()));

			t = new Thread (delegate (object state) {
				object[] args = (object[])state;

				try {
					string data2 = wc.UploadString ((Uri)args[0], (string)args[1], (string)args[2]);
					CreateSession_UploadStringCompleted (this, new UploadCompletedEventArgs (data2, null, false, args[3]));
				} catch (ThreadInterruptedException) {
					CreateSession_UploadStringCompleted (this, new UploadCompletedEventArgs (null, null, true, args[3]));
				} catch (Exception e) {
					CreateSession_UploadStringCompleted (this, new UploadCompletedEventArgs (null, e, false, args[3]));
				}
			});
			
			object[] cb_args = new object[] { Connection.CreateUri (url), null, "", null };
			t.Start (cb_args);
		}
		
		private void CreateSession_UploadStringCompleted (object sender, UploadCompletedEventArgs e)
		{
			if (CreateSessionCompleted != null) {
				if (e.Cancelled || e.Error != null)
					CreateSessionCompleted (this, new CreateSessionEventArgs (e.Cancelled, e.Error, e.UserState));
				else
					CreateSessionCompleted (this, new CreateSessionEventArgs (e.Result, e.UserState));
			}
		}

		public event EventHandler<CreateSessionEventArgs> CreateSessionCompleted;
		#endregion

		#region GetSession
		public Session GetSession (string id)
		{
			string url = string.Format ("/session/{0}", id);

			XmlDocument response = Connection.HttpGet (url);

			if (response.DocumentElement.GetAttribute ("type") == "exception")
				return null;
				
			return new Session (response.DocumentElement);
		}
		#endregion
		
		
		private static string OutputSessionType (SessionType type)
		{
			switch (type) {
				case SessionType.Run: return "run";
				case SessionType.RunDebugger: return "debug";
				case SessionType.RunPackager: return "package";
			}

			throw new ArgumentOutOfRangeException (string.Format ("Unsupported SessionType: {0}", type));
		}
	}
}
