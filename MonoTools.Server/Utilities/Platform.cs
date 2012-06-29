using System;
using System.Runtime.InteropServices;

namespace MonoTools.Server
{
	public static class Platform
	{
		public static OS GetPlatform ()
		{
			switch (Environment.OSVersion.Platform) {
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
					return OS.Windows;
				case PlatformID.MacOSX:
					return OS.Mac;
			}

			if (IsMac ())
				return OS.Mac;
			else
				return OS.Linux;
		}

		// This is the string that is used by the rest server
		public static string GetPlatformString ()
		{
			switch (GetPlatform ()) {
				case OS.Unknown:
				default:
					return "unknown";
				case OS.Linux:
					return "linux";
				case OS.Mac:
					return "mac";
				case OS.Windows:
					return "win";
			}
		}

		public static int GetCapabilities ()
		{
			// Flags
			// RunRemotely = 1,
			// DebugRemotely = 2,
			// Package = 4
			switch (GetPlatformString ()) {
				case "linux":
					return 7;
				default:
					return 1;
			}
		}

		#region Private Methods
		[DllImport ("libc")]
		private static extern int uname (IntPtr buf);

		private static bool IsMac ()
		{
			IntPtr buf = Marshal.AllocHGlobal (8192);

			try {
				// This is a hacktastic way of getting sysname from uname ()
				if (uname (buf) != 0) {
					// WTF: We cannot run uname
					return false;
				} else {
					string os = Marshal.PtrToStringAnsi (buf);

					if (os == "Darwin")
						return true;
					else
						return false;
				}
			} finally {
				Marshal.FreeHGlobal (buf);
			}
		}
		#endregion
	}
}
