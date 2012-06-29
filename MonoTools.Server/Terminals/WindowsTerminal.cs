using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MonoTools.Server
{
	class WindowsTerminal : BaseTerminal
	{
		public override IProcessAsyncOperation StartConsoleProcess (string command, string arguments, string workingDirectory, IDictionary<string, string> environmentVariables, string title, bool pauseWhenFinished)
		{
			string args = "/C \"title " + title + " && \"" + command + "\" " + arguments;
			if (pauseWhenFinished)
				args += " && pause\"";
			else
				args += "\"";

			var psi = new ProcessStartInfo ("cmd.exe", args) {
				CreateNoWindow = true,
				WorkingDirectory = workingDirectory
			};

			if (environmentVariables != null)
				foreach (var env in environmentVariables)
					psi.EnvironmentVariables[env.Key] = env.Value;

			ProcessWrapper proc = new ProcessWrapper ();
			proc.StartInfo = psi;
			proc.Start ();
			return proc;
		}
	}
}
