using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTools.Client
{
	public class UploadCompletedEventArgs
	{
		public Exception Error { get; private set; }
		public string Result { get; private set; }
		public object UserState { get; private set; }
		public bool Cancelled { get; private set; }
		
		internal UploadCompletedEventArgs (string result, Exception error, bool cancelled, object state)
		{
			Result = result;
			Error = error;
			Cancelled = cancelled;
			UserState = state;
		}
	}
}
