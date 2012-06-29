using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.XPath;

namespace MonoTools.Client
{
	public class XspOptions
	{
		public bool UseRandomPort {
			get { return Port == null && BindRange == null; }
		}

		public IPAddress BindAddress {
			get; private set;
		}

		public int? Port {
			get; private set;
		}

		public int[] BindRange {
			get; private set;
		}

		public bool? CustomErrorsOff {
			get; set;
		}

		public string LocalDirectory {
			get; set;
		}

		public XspOptions (IPAddress address, int port)
		{
			this.BindAddress = address;
			this.Port = port;
		}

		public XspOptions (IPAddress address, int port_from, int port_to)
		{
			this.BindAddress = address;
			this.BindRange = new int[] { port_from, port_to };
		}

		public XspOptions (IPAddress address, int port, int port_from, int port_to)
		{
			this.BindAddress = address;
			this.Port = port;
			this.BindRange = new int[] { port_from, port_to };
		}

		public XspOptions (XmlNode node)
		{
			BindAddress = IPAddress.Any;

			foreach (XmlNode item in node) {
				if (item.Name == "port") {
					Port = Int32.Parse (item.InnerText);
				} else if (item.Name == "bindaddress") {
					IPAddress address;
					if (!IPAddress.TryParse (item.InnerText, out address))
						address = Dns.GetHostAddresses (item.InnerText)[0];
					if (address.Equals (IPAddress.Any))
						address = IPAddress.Any;
					BindAddress = address;
				} else if (item.Name == "bindrange") {
					var bind_from = item.SelectSingleNode ("from");
					var bind_to = item.SelectSingleNode ("to");
					BindRange = new int[] { Int32.Parse (bind_from.InnerText), Int32.Parse (bind_to.InnerText) };
				} else if (item.Name == "customerrorsoff") {
					CustomErrorsOff = Boolean.Parse (item.InnerText);
				} else if (item.Name == "localdir") {
					LocalDirectory = item.InnerText;
				}
			}
		}

		public void Serialize (XmlWriter writer)
		{
			writer.WriteStartElement ("xsp-options");
			if (BindAddress != null)
				writer.WriteElementString ("bindaddress", BindAddress.ToString ());
			if (Port != null)
				writer.WriteElementString ("port", ((int)Port).ToString ());
			if (BindRange != null) {
				writer.WriteStartElement ("bindrange");
				writer.WriteElementString ("from", BindRange[0].ToString ());
				writer.WriteElementString ("to", BindRange[1].ToString ());
				writer.WriteEndElement ();
			}
			if (CustomErrorsOff != null)
				writer.WriteElementString ("customerrorsoff", ((bool)CustomErrorsOff).ToString ());
			if (LocalDirectory != null)
				writer.WriteElementString ("localdir", LocalDirectory);
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
