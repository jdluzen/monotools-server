using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Xml;

namespace MonoTools.Server
{
	public class RunExecutableRemotelySession : BaseSession
	{
		public RunExecutableRemotelySession (NameValueCollection query) : base (query)
		{
		}

		public override void Start (HttpListenerContext context)
		{
			Logger.LogDebug ("Starting executable");
			XmlDocument options = context.GetRequestDataAsXml ();

			async_process = StartProcess (options);
			async_process.Completed += new OperationHandler (Process_Exited);

			Logger.LogDebug ("Starting executable completed");

			context.ReturnSuccess ();
		}

		protected virtual IProcessAsyncOperation StartProcess (XmlDocument options)
		{
			string mono = Utilities.GetMonoExecutable ();
			string executable = options.DocumentElement["executable"].InnerText;
			string arguments = options.DocumentElement["arguments"].InnerText;

			string mono_arguments = string.Empty;

			if (options.DocumentElement["monoarguments"] != null)
				mono_arguments = options.DocumentElement["monoarguments"].InnerText;

			List<string> args = new List<string> ();

			AddMonoArguments (args);
			args.Add (mono_arguments);
			args.Add (executable);
			AddExecutableArguments (args);
			args.Add (arguments);

			string args_string = string.Join (" ", args.ToArray ());

			var env_vars = ParseEnvironmentVariables (options);

			Logger.LogDebug ("  Process->File: {0}", mono);
			Logger.LogDebug ("  Process->Arguments: {0}", args_string);
			Logger.LogDebug ("  Process->WorkingDir: {0}", RemotePath);
			Logger.LogDebug ("  Pause Terminal: {0}", PauseTerminal);

			IProcessAsyncOperation console = TerminalManager.StartConsoleProcess (mono, args_string, RemotePath, env_vars, "MonoTools Process", PauseTerminal);
			return console;
		}

		protected virtual void AddMonoArguments (List<string> args)
		{
			args.Add ("--debug");
		}

		protected virtual void AddExecutableArguments (List<string> args)
		{
		}

		protected virtual bool PauseTerminal { get { return MonoToolsConfigurationManager.PauseTerminal; } }
	}
}
