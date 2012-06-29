using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.XPath;

namespace MonoTools.Client
{
	public class XspResult
	{
		public IPEndPoint EndPoint {
			get; private set;
		}

		public string ErrorMessage {
			get; private set;
		}

		public string Root {
			get; private set;
		}

		public bool Success
		{
			get { return EndPoint != null && ErrorMessage == null; }
		}

		public string Url {
			get {
				if (!Success)
					throw new InvalidOperationException ();

				return String.Format ("http://{0}:{1}{2}", EndPoint.Address, EndPoint.Port, Root);
			}
		}

		public XspResult (IPEndPoint endpoint)
		{
			this.EndPoint = endpoint;
			this.Root = "/";
		}

		public XspResult (string error)
		{
			this.ErrorMessage = error;
		}

		public XspResult (XmlNode node)
		{
			foreach (XmlNode item in node) {
				if (item.Name == "root")
					Root = item.InnerText;
				else if (item.Name == "error")
					ErrorMessage = item.InnerText;
				else if (item.Name == "bindaddress") {
					var text = item.InnerText;
					var pos = text.IndexOf (':');
					var address = IPAddress.Parse (text.Substring (0, pos));
					var port = Int32.Parse (text.Substring (pos+1));
					EndPoint = new IPEndPoint (address, port);
				}
			}
		}

		public void Serialize (XmlWriter writer)
		{
			writer.WriteStartElement ("xsp-result");
			writer.WriteElementString ("success", Success ? "true" : "false");
			if (EndPoint != null) {
				writer.WriteElementString ("bindaddress", EndPoint.ToString ());
				writer.WriteElementString ("root", Root);
				writer.WriteElementString ("url", String.Format ("http://{0}/", EndPoint));
			}
			if (ErrorMessage != null)
				writer.WriteElementString ("error", ErrorMessage);
			writer.WriteEndElement ();
		}

		public string ToXml ()
		{
			using (StringWriter sw = new StringWriter ()) {
				using (XmlWriter xw = new XmlTextWriter (sw)) {
					Serialize (xw);
				}
				return sw.ToString ();
			}
		}
	}
}
