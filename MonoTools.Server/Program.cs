using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace MonoTools.Server
{
	partial class RemoteServer
	{
		private static ManualResetEvent wait_for_quit = new ManualResetEvent (false);
		private static Mono.Ssdp.Server ssdp_service;

		private static bool use_ssdp;
		private static bool use_mdb = true;
		private static int port;

		static void Main (string[] args)
		{
			MonoToolsServer server = new MonoToolsServer ();

			use_ssdp = MonoToolsConfigurationManager.UseSsdp;
			port = MonoToolsConfigurationManager.ServerPort;

			OptionSet opts = new OptionSet ();
			bool show_help = false;

			opts.Add ("v", "Turn on debug messages", v => server.SetLogLevel (EnabledLoggingLevel.All));
			opts.Add ("port=", "The port to listen on [default is 8805]", (int v) => port = v);
			opts.Add ("no-ssdp", "Disable Ssdp broadcasting", v => use_ssdp = false);
			opts.Add ("no-mdb", "Disable the mdb debugger support", v => use_mdb = false);
			opts.Add ("h|?|help", "Display this message", v => show_help = true);

			List<string> leftovers = opts.Parse (args);
			
			if (leftovers.Count > 0) {
				Console.WriteLine ();
				Console.WriteLine ("Unknown options:");

				leftovers.ForEach (p => Console.WriteLine ("  {0}", p));
				Console.WriteLine ();
			}

			if (show_help || leftovers.Count > 0) {
				Console.WriteLine ("Usage: monotools-server [OPTIONS]");
				Console.WriteLine ("Runs the server needed for remote operations for MonoTools for Visual Studio.");
				Console.WriteLine ();
				Console.WriteLine ("Options:");

				opts.WriteOptionDescriptions (Console.Out);
				return;
			}

			server.Port = port;

			try {
				server.Start ();
			} catch (System.Net.Sockets.SocketException) {
				Console.WriteLine ("Port {0} is in use, you can specify a different port with: -port=XXXX", port);
				return;
			} catch (HttpListenerException) {
				Console.WriteLine ("Port {0} is in use, you can specify a different port with: -port=XXXX", port);
				return;
			}


			if (use_ssdp)
				StartSsdp ();

			StartInterface ();
			Environment.Exit (0);
		}

		private static void StartSsdp ()
		{
			try {
				string BindAddress = Dns.GetHostName ();
				
				IPAddress ip = null;
				IPAddress[] ips = Dns.GetHostAddresses (BindAddress);
				
				// Try to send an IPv4 address
				foreach (var i in ips)
					if (i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
						ip = i;
						break;
					}

				if (ip == null)
					ip = ips[0];

				// Start the announce server
				ssdp_service = new Mono.Ssdp.Server ();

				string location = string.Format ("{0}|{1}|{2}|{3}|{4}", Dns.GetHostName (), ip.ToString (), port, Platform.GetPlatformString (), Platform.GetCapabilities ());
				ssdp_service.Announce ("mono-vs", Guid.NewGuid ().ToString (), location);
			} catch {
				Logger.LogInfo ("Could not start Ssdp, automatic server detection will be disabled.");
			}
		}

		private static void StartRemotingServer ()
		{
			string mono_executable = Utilities.GetMonoExecutable ();
			string remoting_server = "monovs-server.exe";

			remoting_server = Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().CodeBase), remoting_server);

			List<string> arguments = new List<string> ();

			arguments.Add ("-port=8806");
			arguments.Add (remoting_server);
		}
	}
}
