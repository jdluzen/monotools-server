using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MonoTools.Server
{
	class GnomeTerminal : BaseTerminal
	{
		delegate string TerminalRunnerHandler (string command, string args, string dir, string title, bool pause);

		string terminal_command;
		bool terminal_probed;
		TerminalRunnerHandler runner;

		public override IProcessAsyncOperation StartConsoleProcess (string command, string arguments, string workingDirectory, IDictionary<string, string> environmentVariables, string title, bool pauseWhenFinished)
		{
			ProbeTerminal ();

			string exec = runner (command, arguments, workingDirectory, title, pauseWhenFinished);

			var psi = new ProcessStartInfo (terminal_command, exec) {
				CreateNoWindow = true,
				UseShellExecute = false,
			};

			if (environmentVariables != null)
				foreach (var env in environmentVariables)
					psi.EnvironmentVariables[env.Key] = env.Value;

			ProcessWrapper proc = new ProcessWrapper ();
			proc.StartInfo = psi;
			proc.Start ();
			return proc;
		}

		#region Terminal runner implementations

		private static string GnomeTerminalRunner (string command, string args, string dir, string title, bool pause)
		{
			string extra_commands = pause
				? BashPause.Replace ("'", "\\\"")
				: String.Empty;

			return String.Format (@" --disable-factory --title ""{4}"" -e ""bash -c 'cd {3} ; {0} {1} ; {2}'""",
				command,
				EscapeArgs (args),
				extra_commands,
				EscapeDir (dir),
				title);
		}

		private static string XtermRunner (string command, string args, string dir, string title, bool pause)
		{
			string extra_commands = pause
				? BashPause
				: String.Empty;

			return String.Format (@" -title ""{4}"" -e bash -c ""cd {3} ; '{0}' {1} ; {2}""",
				command,
				EscapeArgs (args),
				extra_commands,
				EscapeDir (dir),
				title);
		}

		private static string EscapeArgs (string args)
		{
			return args.Replace ("\\", "\\\\").Replace ("\"", "\\\"");
		}

		private static string EscapeDir (string dir)
		{
			return dir.Replace (" ", "\\ ");
		}

		private static string BashPause
		{
			get { return @"echo; read -p 'Press any key to continue...' -n1;"; }
		}

		#endregion

		#region Probing for preferred terminal

		private void ProbeTerminal ()
		{
			if (terminal_probed) {
				return;
			}

			terminal_probed = true;

			string fallback_terminal = "xterm";
			string preferred_terminal;
			TerminalRunnerHandler preferred_runner = null;
			TerminalRunnerHandler fallback_runner = XtermRunner;

			if (!String.IsNullOrEmpty (Environment.GetEnvironmentVariable ("GNOME_DESKTOP_SESSION_ID"))) {
				preferred_terminal = "gnome-terminal";
				preferred_runner = GnomeTerminalRunner;
			} else {
				preferred_terminal = fallback_terminal;
				preferred_runner = fallback_runner;
			}

			terminal_command = FindExec (preferred_terminal);
			if (terminal_command != null) {
				runner = preferred_runner;
				return;
			}

			FindExec (fallback_terminal);
			runner = fallback_runner;
		}

		private string FindExec (string command)
		{
			foreach (string path in GetExecPaths ()) {
				string full_path = Path.Combine (path, command);
				try {
					FileInfo info = new FileInfo (full_path);
					// FIXME: System.IO is super lame, should check for 0755
					if (info.Exists) {
						return full_path;
					}
				} catch {
				}
			}

			return null;
		}

		private string[] GetExecPaths ()
		{
			string path = Environment.GetEnvironmentVariable ("PATH");
			if (String.IsNullOrEmpty (path)) {
				return new string[] { "/bin", "/usr/bin", "/usr/local/bin" };
			}

			// this is super lame, should handle quoting/escaping
			return path.Split (':');
		}

		#endregion
	}
}
