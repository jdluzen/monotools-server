using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MonoTools.Client
{
	public class CreateSessionEventArgs : EventArgs
	{
		public bool Cancelled { get; set; }
		public Exception Error { get; set; }
		public XmlDocument Response { get; private set; }
		public object State { get; private set; }
		public Session Session { get; private set; }

		public CreateSessionEventArgs (XmlDocument response, object state)
		{
			Response = response;
			State = state;
			Session = new Session (Response.DocumentElement);
		}

		public CreateSessionEventArgs (string response, object state)
		{
			Response = new XmlDocument ();
			Response.LoadXml (response);
			
			State = state;
			Session = new Session (Response.DocumentElement);
		}

		public CreateSessionEventArgs (bool cancelled, Exception error, object state)
		{
			Cancelled = cancelled;
			Error = error;
			State = state;
		}
	}
}
