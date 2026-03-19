using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.Debug.Logging {
	public struct LogEntry {
		public DateTime timestamp;
		public LogLevel level;
		public StackFrame stackFrame;
		public Thread thread;
		public string message;
		public object[] args;
		public UnityEngine.Object? context;
		public Exception? exception;
	}
}
