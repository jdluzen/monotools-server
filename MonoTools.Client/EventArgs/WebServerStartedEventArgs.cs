using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTools.Client
{
	public class WebServerStartedEventArgs : EventArgs
	{
		public XspResult Result
		{
			get;
			private set;
		}

		public WebServerStartedEventArgs (XspResult result)
		{
			this.Result = result;
		}
	}
}
