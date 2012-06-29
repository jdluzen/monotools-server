using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTools.Server
{
	static class Logger
	{
		static Logger ()
		{
			EnabledLevel = EnabledLoggingLevel.UpToWarn;
		}

		public static EnabledLoggingLevel EnabledLevel { get; set; }

		public static void Log (LogLevel level, string message)
		{
			EnabledLoggingLevel l = (EnabledLoggingLevel)level;
				if ((EnabledLevel & l) == l)
					Console.WriteLine (message); ;
		}

		public static void LogDebug (string messageFormat, params object[] args)
		{
			Log (LogLevel.Debug, string.Format (messageFormat, args));
		}

		public static void LogInfo (string messageFormat, params object[] args)
		{
			Log (LogLevel.Info, string.Format (messageFormat, args));
		}

		public static void LogWarning (string messageFormat, params object[] args)
		{
			Log (LogLevel.Warn, string.Format (messageFormat, args));
		}

		public static void LogError (string messageFormat, params object[] args)
		{
			Log (LogLevel.Error, string.Format (messageFormat, args));
		}

		public static void LogFatalError (string messageFormat, params object[] args)
		{
			Log (LogLevel.Fatal, string.Format (messageFormat, args));
		}
	}
}
