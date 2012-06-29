// Accepts incoming connections from remote systems, and launches
// the debugger
//
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Gdk;
using Gtk;
using Mono.Unix;

namespace MonoTools.Server
{
	partial class RemoteServer
	{
		static StatusIcon status_icon;
		static Menu status_menu;
		static LoggerListener logger;
		static bool exit_application;

		public static void StartInterface ()
		{
			Application.Init ();

			Catalog.Init ("monovs", "./locale");

			logger = new LoggerListener ();
			Debug.Listeners.Add (logger);

			RunInTerminal = MonoToolsConfigurationManager.PauseTerminal;

			// Creation of the status icon
			string icon_resource = "monotools.png";
			
			if (Platform.GetPlatform () == OS.Mac)
				icon_resource = "mactools.png";

			status_icon = new StatusIcon (new Pixbuf (Assembly.GetExecutingAssembly (), icon_resource));
			status_icon.PopupMenu += OnStatusIconPopup;
			status_icon.Visible = true;

			// Creation of status menu
			status_menu = new Menu ();

			// IP Address / Port label
			StringBuilder sb = new StringBuilder ();
			sb.Append ("Listening on:");

			int port = MonoToolsConfigurationManager.ServerPort;

			foreach (IPAddress ip in Dns.GetHostAddresses (Dns.GetHostName ()))
				if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					sb.AppendFormat ("\n{0}:{1}", ip, port);

			LabelMenuItem ip_label = new LabelMenuItem (sb.ToString ());
			ip_label.Activated += HandleIPLabelActivated;
			status_menu.Add (ip_label);

			// Separator
			status_menu.Add (new SeparatorMenuItem ());

			// Run in Terminal
			CheckMenuItem terminal = new CheckMenuItem ("Pause Terminal");
			terminal.Active = RunInTerminal;
			terminal.Activated += HandleTerminalActivated;
			status_menu.Add (terminal);

			// Log viewer
			ImageMenuItem menu_log_viewer = new ImageMenuItem (Catalog.GetString ("Log _Viewer"));
			menu_log_viewer.Image = new Gtk.Image (Stock.Info, IconSize.Menu);
			menu_log_viewer.Activated += OnLogViewerMenuActivated;
			status_menu.Add (menu_log_viewer);

			// About
			ImageMenuItem menu_about = new ImageMenuItem (Catalog.GetString ("_About"));
			menu_about.Image = new Gtk.Image (Stock.About, IconSize.Menu);
			menu_about.Activated += OnAboutMenuActivated;
			status_menu.Add (menu_about);

			// Separator
			status_menu.Add (new SeparatorMenuItem ());

			// Quit
			ImageMenuItem menu_quit = new ImageMenuItem (Catalog.GetString ("_Quit"));
			menu_quit.Image = new Gtk.Image (Stock.Quit, IconSize.Menu);
			menu_quit.Activated += OnQuitMenuActivated;
			status_menu.Add (menu_quit);

			status_menu.ShowAll ();

			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				Thread signal_thread = new Thread (delegate () {

					UnixSignal[] signals = new UnixSignal[] {
					new UnixSignal (Mono.Unix.Native.Signum.SIGINT),
					new UnixSignal (Mono.Unix.Native.Signum.SIGTERM),
				};

					while (!exit_application && (UnixSignal.WaitAny (signals, 1000) == 1000)) {
					}

					Application.Quit ();
				});

				signal_thread.Start ();
			}

			Application.Run ();
			//StopServer ();
		}


		private static void HandleIPLabelActivated (object sender, EventArgs e)
		{
			Label l = (sender as LabelMenuItem).Label;

			Clipboard cb = Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false));

			// Chop off: "Listening on:"
			if (l.Text.Length > 13)
				cb.Text = l.Text.Substring (13).Trim (' ', '\n');
		}

		public static bool RunInTerminal
		{
			get;
			set;
		}

		private static void OutputHandler (object process, DataReceivedEventArgs output)
		{
			logger.WriteLine (output.Data);
		}


		private static void OnStatusIconPopup (object o, PopupMenuArgs args)
		{
			status_menu.Popup ();
		}

		private static void OnLogViewerMenuActivated (object sender, EventArgs e)
		{
			logger.ShowDialog ();
		}

		private static void OnAboutMenuActivated (object sender, EventArgs e)
		{
			AboutDialog about = new AboutDialog ();
			about.Logo = new Pixbuf (Assembly.GetExecutingAssembly (), "monotools.png");
			about.Icon = about.Logo;
			about.Version = Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			about.ProgramName = Catalog.GetString ("MonoTools");
			about.Website = "http://www.go-mono.com/monovs/";
			about.WebsiteLabel = Catalog.GetString ("Visit Homepage");
			about.Copyright = Catalog.GetString ("Copyright \xa9 2010 Novell, Inc.");
			about.Authors = null;
			about.Documenters = null;
			about.TranslatorCredits = null;
			about.Run ();
			about.Destroy ();
		}

		private static void OnQuitMenuActivated (object sender, EventArgs e)
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
				exit_application = true;
			else
				Application.Quit ();
		}

		private static void HandleTerminalActivated (object sender, EventArgs e)
		{
			RunInTerminal = (sender as CheckMenuItem).Active;
			MonoToolsConfigurationManager.PauseTerminal = (sender as CheckMenuItem).Active;
			MonoToolsConfigurationManager.Save ();
		}
	}
}