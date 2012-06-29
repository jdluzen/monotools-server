using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTools.Server
{
	public enum LogLevel
	{
		Fatal = 1,
		Error = 2,
		Warn = 4,
		Info = 8,
		Debug = 16
	}

	[Flags]
	public enum EnabledLoggingLevel
	{
		Fatal = 1,
		Error = 2,
		Warn = 4,
		Info = 8,
		Debug = 16,

		None = 0,

		UpToFatal = Fatal,
		UpToError = Error | UpToFatal,
		UpToWarn = Warn | UpToError,
		UpToInfo = Info | UpToWarn,
		UpToDebug = Debug | UpToInfo,

		All = UpToDebug
	}
}
