using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


#nullable enable

namespace SoulboundBackend.Core.Debug {
	public static class Log {
		public static void Info(string message) => Log_Skip2Frames(LogLevel.Info, message);
		public static void Warn(string message) => Log_Skip2Frames(LogLevel.Warning, message);
		public static void Error(string message) => Log_Skip2Frames(LogLevel.Error, message);
		public static void Fatal(string message) => Log_Skip2Frames(LogLevel.Fatal, message);

		private static void _Log(
			LogLevel level,
			string message,
			StackFrame stackFrame,
			Exception? exception = null,
			UnityEngine.GameObject? context = null,
			params object[] args
		) {
			SoulboundDebug.instance.GetLogHandler().Log(new LogEntry {
				timestamp = DateTime.Now,
				level = level,
				stackFrame = stackFrame,
				thread = Thread.CurrentThread,
				message = message,
				args = args,
				exception = exception,
				context = context
			});
		}

		private static void Log_Skip2Frames(
			LogLevel level,
			string message,
			Exception? exception = null,
			UnityEngine.GameObject? context = null,
			params object[] args
		) => _Log(level, message, NewStackFrame(2), exception, context, args);

		private static StackFrame NewStackFrame(int skipFrames) {
			return new StackFrame(skipFrames, false);
		}
	}
}
