using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core.Debug {
	public sealed class SoulboundLogHandler {
		const string argEntryMarker = "{}";
		private readonly ILogHandler logHandler;

		public SoulboundLogHandler(ILogHandler logHandler) {
			this.logHandler = logHandler;
		}

		public void Log(LogEntry entry) {
			string message = GetFinalMessage(entry);
			LogType unityLogType = ToUnityLogType(entry.level);

			logHandler.LogFormat(unityLogType, entry.context, message);

			if (entry.exception != null) {
				logHandler.LogException(entry.exception, entry.context);
			}
		}

		private string GetFinalMessage(LogEntry entry) {
			string message = PlaceArgs(entry.message, argEntryMarker, entry.args);
			string timestamp = FormatTimestamp(entry.timestamp);
			string thread = FormatThread(entry.thread);
			string level = FormatLevel(entry.level);
			string stackFrame = FormatStackFrame(entry.stackFrame);

			// [{time}] [{level}] [{thread}/{stackFrame}]: {message}
			const string logFormat = "[{0}] [{1}] [{2}/{3}]: {4}";

			string formatted = string.Format(logFormat,
				timestamp,
				level,
				thread, 
				stackFrame,
				message
			);
			return formatted;
		}

		private LogType ToUnityLogType(LogLevel level) {
			return level switch {
				LogLevel.Info => LogType.Log,
				LogLevel.Warning => LogType.Warning,
				LogLevel.Error => LogType.Error,
				LogLevel.Fatal => LogType.Exception,
				_ => throw new ArgumentException("Invalid log level: " + level)
			};
		}

		private string FormatTimestamp(DateTime dateTime) {
			string timestamp = dateTime.ToString();
			return timestamp;
		}

		private string FormatThread(Thread thread) {
			int id = thread.ManagedThreadId;
			string formatted = $"thread-{id}";

			if (!string.IsNullOrEmpty(thread.Name)) {
				formatted = $"{formatted}({thread.Name})";
			}

			return formatted;
		}

		private string FormatLevel(LogLevel level) {
			return level.ToString().ToUpper();
		}

		private string FormatStackFrame(StackFrame stackFrame) {
			var method = stackFrame.GetMethod();
			var declaringType = method.DeclaringType;
			string caller = declaringType != null ? declaringType.Name : "Unknown";
			return caller;
		}

		private string PlaceArgs(string s, string argEntryMarker, params object[] args) {
			if (string.IsNullOrEmpty(s) || args == null || args.Length == 0) return s;

			int argIndex = 0;
			while (s.Contains(argEntryMarker) && argIndex < args.Length) {
				s = s.ReplaceFirst(argEntryMarker, args[argIndex]?.ToString() ?? "null");
				argIndex++;
			}

			return s;
		}
	}

	//static class StringReplaceFirst {
	//	public static string ReplaceFirst(this string text, string search, string replace) {
	//		int pos = text.IndexOf(search);
	//		if (pos < 0) {
	//			return text;
	//		}
	//		return text[..pos] + replace + text[(pos + search.Length)..];
	//	}
	//}
}
