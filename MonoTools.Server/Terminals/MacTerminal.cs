using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTools.Server
{
	class MacTerminal : BaseTerminal
	{
		public override IProcessAsyncOperation StartConsoleProcess (string command, string arguments, string workingDirectory, IDictionary<string, string> environmentVariables, string title, bool pauseWhenFinished)
		{
			return new ExternalConsoleProcess (command, arguments, workingDirectory, environmentVariables, title, pauseWhenFinished);
		}
	}
}
