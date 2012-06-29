using System;
using System.Net;
using System.Xml;
using Mono.WebServer;
using MonoTools.Client;

namespace MonoTools.WebServer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			XmlDocument doc = new XmlDocument ();
			doc.Load (Console.In);

			XmlNode node = doc.SelectSingleNode ("/xsp-options");

			var opts = new XspOptions (node);

			var source = new MonoWebSource (opts);

			Environment.CurrentDirectory = opts.LocalDirectory;

			var server = new ApplicationServer (source);
			server.AddApplicationsFromCommandLine ("/:.");

			XspResult result;

			try {
				if (server.Start (false)) {
					result = new XspResult (source.EndPoint);
				} else {
					result = new XspResult ("Cannot start xsp.");
				}
			} catch (Exception ex) {
				var msg = String.Format ("Cannot start xsp: {0}", ex.Message);
				result = new XspResult (msg);
			}

			Console.Out.WriteLine (result.ToXml ());
			Console.Out.Flush ();
		}
	}
}
