namespace SoulboundEngine.Logging {
	using System;
	using System.Diagnostics;
	using System.Threading;

#nullable enable

	public struct LogEntry {
		public DateTime timestamp;
		public LogLevel level;
		public StackFrame stackFrame;
		public Thread thread;
		public string message;
		public object[] args;
		public Exception? exception;
	}
}
