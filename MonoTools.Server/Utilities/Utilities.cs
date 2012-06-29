using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml;
using Microsoft.Win32;

namespace MonoTools.Server
{
	public static class Utilities
	{
		public static string GetMonoExecutable ()
		{
			if (Platform.GetPlatform () == OS.Windows)
				return Path.Combine (Path.Combine (GetWindowsMonoPath (), "bin"), "mono.exe");
			else
				return Path.Combine (Path.Combine (GetUnixMonoPath (), "bin"), "mono");
		}

		public static string GetXspExecutable ()
		{
			if (Platform.GetPlatform () == OS.Windows) {
				string path = Path.Combine (Path.Combine (GetWindowsMonoPath (), "lib"), "mono");
				path = Path.Combine (path, "2.0");
				path = Path.Combine (path, "winhack");
				path = Path.Combine (path, "xsp2.exe");
				return path;
			} else {
				// /usr/lib/mono/2.0/xsp2.exe
				string path = Path.Combine (Path.Combine (GetUnixMonoPath (), "lib"), "mono");
				path = Path.Combine (path, "2.0");
				path = Path.Combine (path, "xsp2.exe");
				return path;
			}
				//return Path.Combine (Path.Combine (GetUnixMonoPath (), "bin"), "xsp2");
		}

		public static string GetWindowsMonoPath ()
		{
			string default_clr;
			string mono_path;

			// Look in the default location
			default_clr = (string)Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Novell\Mono", "DefaultCLR", "NONE");

			// If we didn't find it, look in the 64bit location
			if (string.IsNullOrEmpty (default_clr) || default_clr == "NONE")
				default_clr = (string)Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Novell\Mono", "DefaultCLR", "NONE");

			// If we still didn't find it, throw an exception
			if (string.IsNullOrEmpty (default_clr) || default_clr == "NONE")
				return null;

			// Look for the path to the mono runtime
			mono_path = (string)Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Novell\Mono\" + default_clr, "SdkInstallRoot", "NONE");

			// If we didn't find it, look in the 64bit location
			if (string.IsNullOrEmpty (mono_path) || mono_path == "NONE")
				mono_path = (string)Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Novell\Mono\" + default_clr, "SdkInstallRoot", "NONE");

			// If we still didn't find it, throw an exception
			if (string.IsNullOrEmpty (mono_path) || mono_path == "NONE")
				return null;

			return mono_path;
		}

		private static string GetUnixMonoPath ()
		{
			// Use relative path based on where mscorlib.dll is at to enable relocation
			return new DirectoryInfo (Path.GetDirectoryName (typeof (int).Assembly.Location)).Parent.Parent.Parent.FullName;
		}

		public static SessionType ParseSessionType (string type)
		{
			switch (type.ToLowerInvariant ()) {
				case "run":
					return SessionType.Run;
				case "debug":
					return SessionType.RunDebugger;
				case "package":
					return SessionType.RunPackager;
			}

			throw new ArgumentOutOfRangeException (string.Format ("Unsupported SessionType: {0}", type));
		}

		public static string OutputSessionType (SessionType type)
		{
			switch (type) {
				case SessionType.Run:
					return "run";
				case SessionType.RunDebugger:
					return "debug";
				case SessionType.RunPackager:
					return "package";
			}

			throw new ArgumentOutOfRangeException (string.Format ("Unsupported SessionType: {0}", type));
		}


		private static HashAlgorithm hashAlg = SHA1.Create ();

		public static string CreateHash (string filename)
		{
			using (Stream file = new FileStream (filename, FileMode.Open, FileAccess.Read))
				return BitConverter.ToString (hashAlg.ComputeHash (file)).Replace ("-", "");
		}

		public static void WriteExceptionToXml (XmlWriter writer, Exception ex)
		{
			writer.WriteStartElement ("exception");
			writer.WriteElementString ("type", ex.GetType ().ToString ());
			writer.WriteElementString ("message", ex.Message);
			writer.WriteElementString ("stacktrace", ex.StackTrace);
			writer.WriteElementString ("complete", ex.ToString ());

			if (ex.InnerException != null) {
				writer.WriteStartElement ("innerexception");
				WriteExceptionToXml (writer, ex.InnerException);
				writer.WriteEndElement ();
			}

			writer.WriteEndElement ();
		}

		public static void ModifyWebConfig (string path, bool setDebug, bool turnOffCustomErrors)
		{
			string config = FindWebConfig (path);

			XmlDocument doc = new XmlDocument ();

			// The user doesn't have a web.config, so we create one
			if (string.IsNullOrEmpty (config))
				doc.LoadXml ("<configuration />");
			else
				doc.Load (config);

			XmlElement configuration = doc.DocumentElement;
			XmlElement systemweb = configuration["system.web"];

			if (systemweb == null) {
				systemweb = doc.CreateElement ("system.web");
				configuration.AppendChild (systemweb);
			}

			// Add debug = true
			if (setDebug) {
				XmlElement compilation = systemweb["compilation"];

				if (compilation == null) {
					compilation = doc.CreateElement ("compilation");
					systemweb.AppendChild (compilation);
				}

				compilation.SetAttribute ("debug", "true");
			}

			// Turn off custom errors
			if (turnOffCustomErrors) {
				XmlElement customerrors = systemweb["customErrors"];

				if (customerrors == null) {
					customerrors = doc.CreateElement ("customErrors");
					systemweb.AppendChild (customerrors);
				}

				customerrors.SetAttribute ("mode", "Off");
			}

			if (string.IsNullOrEmpty (config))
				doc.Save (Path.Combine (path, "Web.config"));
			else
				doc.Save (config);
		}

		private static string FindWebConfig (string path)
		{
			foreach (string file in Directory.GetFiles (path))
				if (string.Compare (Path.GetFileName (file), "web.config", true) == 0)
					return file;

			return null;
		}
	}
}
