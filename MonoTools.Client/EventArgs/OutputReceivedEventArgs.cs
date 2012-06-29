using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTools.Client
{
	public class OutputReceivedEventArgs : EventArgs
	{
		public string Output
		{
			get;
			private set;
		}

		public bool IsError
		{
			get;
			private set;
		}

		public OutputReceivedEventArgs (string output, bool is_error)
		{
			this.Output = output;
			this.IsError = is_error;
		}
	}
}
