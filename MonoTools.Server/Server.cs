using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTools.Server
{
	partial class RemoteServer
	{
		public static void StartInterface ()
		{
			// Block until exit
			wait_for_quit.WaitOne ();
		}
	}
}
