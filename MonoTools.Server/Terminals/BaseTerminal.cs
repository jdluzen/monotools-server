using System;
using System.Collections.Generic;

namespace MonoTools.Server
{
	abstract class BaseTerminal
	{
		public abstract IProcessAsyncOperation StartConsoleProcess (string command, string arguments, string workingDirectory, IDictionary<string, string> environmentVariables, string title, bool pauseWhenFinished);
	}
}
