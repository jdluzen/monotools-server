using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

namespace MonoTools.Server
{
	public static class MonoToolsConfigurationManager
	{
		static Configuration config;

		// 8805-8872 are unassigned according to:
		// http://www.iana.org/assignments/port-numbers

		public static int ServerPort { get; private set; }
		public static int ApplicationPortRangeStart { get; set; }
		public static int ApplicationPortRangeEnd { get; set; }

		public static bool UseSsdp { get; set; }
		public static bool PauseTerminal { get; set; }

		static MonoToolsConfigurationManager ()
		{
			try {
				string location = Assembly.GetEntryAssembly ().Location;
				config = ConfigurationManager.OpenExeConfiguration (location);
			} catch {
				// Ignore, fallback to machine.config
				config = null;
			}
			
			if (config == null) {
				if (Environment.OSVersion.Platform == PlatformID.Unix)
					config = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.PerUserRoamingAndLocal);
				else
					config = ConfigurationManager.OpenMachineConfiguration ();
			}
			
			Logger.LogInfo ("Using config file: {0}", config.FilePath);

			ServerPort = GetAppSetting<int> (config, "ServerPort", 8805);
			ApplicationPortRangeStart = GetAppSetting<int> (config, "ApplicationPortRangeStart", 8806);
			ApplicationPortRangeEnd = GetAppSetting<int> (config, "ApplicationPortRangeEnd", 8872);

			UseSsdp = GetAppSetting<bool> (config, "UseSsdp", true);
			PauseTerminal = GetAppSetting<bool> (config, "PauseTerminal", true);

			ReadUserConfigFile ();
		}

		public static void Save ()
		{
			string path = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
			path = Path.Combine (path, "monotools");

			if (!Directory.Exists (path))
				Directory.CreateDirectory (path);

			path = Path.Combine (path, "settings.xml");

			XmlDocument doc = new XmlDocument ();

			// We load the old one and modify it so we can do non-destructive changes
			if (!File.Exists (path))
				doc.LoadXml ("<configuration><appSettings></appSettings></configuration>");
			else
				doc.Load (path);

			WriteUserSetting (doc, "PauseTerminal", PauseTerminal.ToString ());

			doc.Save (path);
		}

		private static T GetAppSetting<T> (System.Configuration.Configuration config, string key, T defaultValue)
		{
			var setting = config.AppSettings.Settings[key];
			
			if (setting == null)
				return defaultValue;
			
			string value = setting.Value;

			if (typeof(T) == typeof(string))
				return (T)(object)value; else if (typeof(T) == typeof(bool))
				return (T)(object)bool.Parse (value); else if (typeof(T) == typeof(int))
				return (T)(object)int.Parse (value);
			
			throw new NotSupportedException ("Unsupported AppSetting type used");
		}

		private static void ReadUserConfigFile ()
		{
			string path = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
			path = Path.Combine (path, "monotools");
			path = Path.Combine (path, "settings.xml");

			if (File.Exists (path)) {
				XmlDocument doc = new XmlDocument ();
				doc.Load (path);

				PauseTerminal = GetUserSetting<bool> (doc, "PauseTerminal", false);
			}
		}

		private static T GetUserSetting<T> (XmlDocument doc, string key, T defaultValue)
		{
			XmlElement xe = (XmlElement)doc.SelectSingleNode (string.Format ("//configuration/appSettings/add[@key = \"{0}\"]", key));

			if (xe == null)
				return defaultValue;

			string value = xe.GetAttribute ("value");

			if (typeof(T) == typeof(string))
				return (T)(object)value; else if (typeof(T) == typeof(bool))
				return (T)(object)bool.Parse (value); else if (typeof(T) == typeof(int))
				return (T)(object)int.Parse (value);

			throw new NotSupportedException ("Unsupported AppSetting type used");
		}

		private static void WriteUserSetting (XmlDocument doc, string key, string val)
		{
			XmlElement xe = (XmlElement)doc.SelectSingleNode (string.Format ("//configuration/appSettings/add[@key = \"{0}\"]", key));

			if (xe != null) {
				xe.SetAttribute ("value", val);
				return;
			}

			XmlElement child = doc.CreateElement ("add");

			child.SetAttribute ("key", key);
			child.SetAttribute ("value", val);

			doc["configuration"]["appSettings"].AppendChild (child);
		}
	}
}