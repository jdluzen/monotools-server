using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MonoTools.Client
{
	//<server>
	//  <version>1.0</version>
	//  <os>Linux</os>
	//  <secureonly>false</secureonly>
	//  <supportsrun>true</supportsrun>
	//  <supportssdb>true</supportssdb>
	//  <supportsmdb>true</supportsmdb>
	//  <supportsepm>true</supportsrpm>
	//</server>
	public class ServerDetails
	{
		public Version Version { get; private set; }
		public string OS { get; private set; }
		public bool RequiresSecurity { get; private set; }
		public bool SupportsRunRemotely { get; private set; }
		public bool SupportsMdb { get; private set; }
		public bool SupportsSdb { get; private set; }
		public bool SupportsRpm { get; private set; }
		
		internal ServerDetails (XmlElement response)
		{
			Version = new Version (response["version"].InnerText);
			OS = response["os"].InnerText;
			RequiresSecurity = bool.Parse (response["secureonly"].InnerText);
			SupportsRunRemotely = bool.Parse (response["supportsrun"].InnerText);
			SupportsMdb = bool.Parse (response["supportssdb"].InnerText);
			SupportsSdb = bool.Parse (response["supportsmdb"].InnerText);
			SupportsRpm = bool.Parse (response["supportsrpm"].InnerText);
		}
	}
}
