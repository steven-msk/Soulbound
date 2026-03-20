
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.Debug.Logging {
	internal sealed class Logger {
		const string ARG_MARKER = "{}";
		const string LOG_FORMAT = "[{0}] [{1}] [{2}/{3}]: {4}";		// [time] [thread] [stackFrame/level]: {message}
		const string TIME_FORMAT = "{0}-{1}-{2} {3}:{4}:{5}.{6}";   // day-month-year hour:minute:second.millis
		
		private static Logger instance { get; set; } = null!;
		private readonly ILogger logger;

		public Logger(ILogger logger) {
			instance = this;
			this.logger = logger;
		}

		private static void LogMessage(
			Action<string> loggingMethod,
			LogLevel level,
			StackFrame stackFrame,
			string message,
			Exception? exception = null,
			UnityEngine.Object? context = null,
			params object[] args
		) {
			LogEntry logEntry = new() {
				message = message,
				level = level,
				args = args,
				stackFrame = stackFrame,
				thread = Thread.CurrentThread,
				timestamp = DateTime.Now,
				exception = exception,
				context = context,
			};
			string finalMessage = GetFinalMessage(logEntry);
			loggingMethod(finalMessage);
			if (exception != null) instance?.logger.LogException(exception, context);
		}

		public static void LogInfo(object message, UnityEngine.Object? context = null) {
			LogMessage(LogInfo_Method, LogLevel.Info, CaptureStackFrame(), message?.ToString() ?? "null", context: context);
		}

		public static void LogInfo(string message, params object[] args) {
			LogMessage(LogInfo_Method, LogLevel.Info, CaptureStackFrame(), message, args: args);
		}

		public static void LogWarning(object message, UnityEngine.Object? context = null) {
			LogMessage(LogWarning_Method, LogLevel.Warning, CaptureStackFrame(), message?.ToString() ?? "null", context: context);
		}

		public static void LogWarning(string message, params object[] args) {
			LogMessage(LogWarning_Method, LogLevel.Warning, CaptureStackFrame(), message, args: args);
		}

		public static void LogError(object message, UnityEngine.Object? context = null) {
			LogMessage(LogError_Method, LogLevel.Error, CaptureStackFrame(), message?.ToString() ?? "null", context: context);
		}

		public static void LogError(string message, params object[] args) {
			LogMessage(LogError_Method, LogLevel.Error, CaptureStackFrame(), message, args: args);
		}

		public static void LogFatal(Exception? exception, object message, UnityEngine.Object? context = null) {
			LogMessage(LogError_Method, LogLevel.Fatal, CaptureStackFrame(), message?.ToString() ?? "null", exception, context);
		}

		public static void LogFatal(Exception? exception, string message, params object[] args) {
			LogMessage(LogError_Method, LogLevel.Fatal, CaptureStackFrame(), message, exception, args: args);
		}

		public static void LogFatal(Exception? exception, UnityEngine.Object? context = null) {
			LogMessage(LogError_Method, LogLevel.Fatal, CaptureStackFrame(), "", exception, context);
		}

		private static void LogInfo_Method(string mesage) => instance?.logger.Log(LogType.Log, mesage);
		private static void LogWarning_Method(string mesage) => instance?.logger.Log(LogType.Warning, mesage);
		private static void LogError_Method(string mesage) => instance?.logger.Log(LogType.Error, mesage);

		private static string GetFinalMessage(LogEntry entry) {
			string message = PlaceArgs(entry.message, ARG_MARKER, entry.args);
			string timestamp = FormatTimestamp(entry.timestamp);
			string thread = FormatThread(entry.thread);
			string level = FormatLevel(entry.level);
			string stackFrame = FormatStackFrame(entry.stackFrame);
			message = FormatException(entry.exception, message);

			string formatted = string.Format(LOG_FORMAT,
				timestamp, thread, stackFrame, level, message
			);
			return formatted;
		}

		private static string FormatTimestamp(DateTime dateTime) {
			string timestamp = string.Format(TIME_FORMAT,
				dateTime.Day, dateTime.Month, dateTime.Year,
				dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond
			);
			return timestamp;
		}

		private static string FormatThread(Thread thread) {
			int id = thread.ManagedThreadId;
			string formatted = $"thread-{id}";

			if (!string.IsNullOrEmpty(thread.Name)) {
				formatted = $"{formatted}({thread.Name})";
			}

			return formatted;
		}

		private static string FormatLevel(LogLevel level) {
			return level.ToString().ToUpper();
		}

		private static string FormatStackFrame(StackFrame stackFrame) {
			var method = stackFrame.GetMethod();
			var declaringType = method.DeclaringType;
			string caller = declaringType != null ? declaringType.Name : "Unknown";
			return caller;
		}

		private static string FormatException(Exception? exception, string message) {
			string exceptionMark = exception != null
				? $"Exception thrown! {exception.GetType().Name}: "
				: string.Empty;
			return exceptionMark + message;
		}

		private static string PlaceArgs(string text, string argMarker, params object[] args) {
			if (string.IsNullOrEmpty(text) || args == null || args.Length == 0) return text;

			int argIndex = 0;
			while (text.Contains(argMarker) && argIndex < args.Length) {
				text = text.ReplaceFirst(argMarker, args[argIndex]?.ToString() ?? "null");
				argIndex++;
			}
			return text;
		}

		private static StackFrame CaptureStackFrame(int skipFrames = 1) {
			return new StackFrame(skipFrames + 1, true);
		} 
	}

	static class StringReplaceFirst {
		public static string ReplaceFirst(this string text, string search, string replace) {
			int pos = text.IndexOf(search);
			if (pos < 0) return text;
			return text[..pos] + replace + text[(pos + search.Length)..];
		}
	}
}
