using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MonoTools.Client
{
	public class ServerResponseEventArgs : EventArgs
	{
		public bool Cancelled { get; set; }
		public Exception Error { get; set; }
		public XmlDocument Response { get; private set; }
		public object State { get; private set; }

		public ServerResponseEventArgs (XmlDocument response, object state)
		{
			Response = response;
			State = state;
		}

		public ServerResponseEventArgs (string response, object state)
		{
			Response = new XmlDocument ();
			Response.LoadXml (response);
			
			State = state;
		}
		
		public ServerResponseEventArgs (bool cancelled, Exception error, object state)
		{
			Cancelled = cancelled;
			Error = error;
			State = state;
		}
	}
}
