using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTools.Client
{
	public class SynchronizeFilesEventArgs : EventArgs
	{
		public SynchonizeStep Step { get; private set; }
		public long Completed { get; private set; }
		public long Total { get; private set; }
		public float PercentComplete { get; private set; }
		public TimeSpan ElapsedTime { get; private set; }

		public SynchronizeFilesEventArgs (SynchonizeStep step, long complete, long total, TimeSpan elapsed)
		{
			Step = step;
			Completed = complete;
			Total = total;
			PercentComplete = ((float)complete / (float)total) * 100;
			ElapsedTime = elapsed;
		}
	}
	
	public enum SynchonizeStep
	{
		CompareWithServer,
		CompressFiles,
		SendFiles
	}
}
