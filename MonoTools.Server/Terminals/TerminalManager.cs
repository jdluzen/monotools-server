using System;
using System.Collections.Generic;

namespace MonoTools.Server
{
	class TerminalManager
	{
		public static IProcessAsyncOperation StartConsoleProcess (string command, string arguments, string workingDirectory, IDictionary<string, string> environmentVariables, string title, bool pauseWhenFinished)
		{
			return GetTerminal ().StartConsoleProcess (command, arguments, workingDirectory, environmentVariables, "MonoTools Session", pauseWhenFinished);
		}

		private static BaseTerminal GetTerminal ()
		{
			switch (Platform.GetPlatform ()) {
				case OS.Mac:
					return new MacTerminal ();
				case OS.Windows:
					return new WindowsTerminal ();
				case OS.Linux:
				default:
					return new GnomeTerminal ();
			}
		}
	}
}
