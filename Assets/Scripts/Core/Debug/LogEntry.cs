using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Core.Debug {
	public struct LogEntry {
		public DateTime timestamp;
		public LogLevel level;
		public StackFrame stackFrame;
		public Thread thread;
		public string message;
		public object[] args;
		public GameObject? context;
		public Exception? exception;
	}
}
