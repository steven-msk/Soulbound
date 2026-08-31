namespace SoulboundEngine.UnityClient.Debug.Logging {
	using SoulboundEngine.Common;
	using SoulboundEngine.Logging;
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Threading;
	using UnityEngine;

#nullable enable

	public class UnityClientLoggerWrapper : ILoggerWrapper {
		const string ARG_MARKER = "{}";
		const string LOG_FORMAT = "[{0}] [{1}] [{2}/{3}]: {4}";     // [time] [thread] [stackFrame/level]: {message}
		const string TIME_FORMAT = "{0}-{1}-{2} {3}:{4}:{5}.{6}";   // day-month-year hour:minute:second.millis
		private readonly ILogger unityLogger;

		public UnityClientLoggerWrapper(ILogger unityLogger) {
			this.unityLogger = unityLogger;
		}

		private void LogMessage(
			Action<string> loggingMethod,
			LogLevel level,
			StackFrame stackFrame,
			string message,
			Exception? exception = null,
			object? context = null,
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
			};
			string finalMessage = GetFinalMessage(logEntry);
			loggingMethod(finalMessage);
			if (exception != null) this.unityLogger.LogException(exception, context as UnityEngine.Object);
		}

		public void LogInfo(object message) {
			this.LogMessage(this.LogInfo_Method, LogLevel.Info, CaptureStackFrame(), message?.ToString() ?? "null", null, null);
		}

		public void LogInfo(object message, object? context) {
			this.LogMessage(this.LogInfo_Method, LogLevel.Info, CaptureStackFrame(), message?.ToString() ?? "null", null, context);
		}

		public void LogInfo(string message, params object[] args) {
			this.LogMessage(this.LogInfo_Method, LogLevel.Info, CaptureStackFrame(), message, args: args);
		}

		public void LogWarning(object message) {
			this.LogMessage(this.LogWarning_Method, LogLevel.Warning, CaptureStackFrame(), message?.ToString() ?? "null", null, null);
		}

		public void LogWarning(object message, object? context) {
			this.LogMessage(this.LogWarning_Method, LogLevel.Warning, CaptureStackFrame(), message?.ToString() ?? "null", null, context);
		}

		public void LogWarning(string message, params object[] args) {
			this.LogMessage(this.LogWarning_Method, LogLevel.Warning, CaptureStackFrame(), message, args: args);
		}

		public void LogError(object message) {
			this.LogMessage(this.LogError_Method, LogLevel.Error, CaptureStackFrame(), message?.ToString() ?? "null", null, null);
		}

		public void LogError(object message, object? context) {
			this.LogMessage(this.LogError_Method, LogLevel.Error, CaptureStackFrame(), message?.ToString() ?? "null", null, context);
		}

		public void LogError(string message, params object[] args) {
			this.LogMessage(this.LogError_Method, LogLevel.Error, CaptureStackFrame(), message, args: args);
		}

		public void LogFatal(Exception? exception, object message) {
			this.LogMessage(this.LogError_Method, LogLevel.Fatal, CaptureStackFrame(), message?.ToString() ?? "null", exception, null);
		}

		public void LogFatal(Exception? exception, object message, object? context) {
			this.LogMessage(this.LogError_Method, LogLevel.Fatal, CaptureStackFrame(), message?.ToString() ?? "null", exception, context);
		}

		public void LogFatal(Exception? exception, string message, params object[] args) {
			this.LogMessage(this.LogError_Method, LogLevel.Fatal, CaptureStackFrame(), message, exception, args: args);
		}

		public void LogFatal(Exception? exception) {
			this.LogMessage(this.LogError_Method, LogLevel.Fatal, CaptureStackFrame(), "", exception, null);
		}

		public void LogFatal(Exception? exception, UnityEngine.Object context) {
			this.LogMessage(this.LogError_Method, LogLevel.Fatal, CaptureStackFrame(), "", exception, context);
		}

		private void LogInfo_Method(string mesage) => this.unityLogger.Log(LogType.Log, mesage);
		private void LogWarning_Method(string mesage) => this.unityLogger.Log(LogType.Warning, mesage);
		private void LogError_Method(string mesage) => this.unityLogger.Log(LogType.Error, mesage);

		private static string GetFinalMessage(LogEntry entry) {
			string message = entry.message.WithArgs(ARG_MARKER, entry.args);
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
			MethodBase? method = stackFrame.GetMethod();
			Type? declaringType = method.DeclaringType;
			string caller = declaringType != null ? declaringType.Name : "Unknown";
			return caller;
		}

		private static string FormatException(Exception? exception, string message) {
			string exceptionMark = exception != null
				? $"Exception thrown! {exception.GetType().Name}: '{exception.Message}'."
				: string.Empty;
			return exceptionMark + message;
		}

		private static string PlaceArgs(string text, string argMarker, params object[] args) {
			if (string.IsNullOrEmpty(text) || args == null || args.Length == 0) {
				return text;
			}

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

}
