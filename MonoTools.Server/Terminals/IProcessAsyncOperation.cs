using System;

namespace MonoTools.Server
{
	public interface IProcessAsyncOperation : IAsyncOperation, IDisposable
	{
		int ExitCode { get; }
		int ProcessId { get; }
	}

	public delegate void OperationHandler (IAsyncOperation op);

	public interface IAsyncOperation
	{
		void Cancel ();
		bool WaitForCompleted (int milliseconds);
		void WaitForCompleted ();
		bool IsCompleted { get; }
		bool Success { get; }
		bool SuccessWithWarnings { get; }

		event OperationHandler Completed;
	}
}
