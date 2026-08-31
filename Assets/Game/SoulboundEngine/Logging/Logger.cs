namespace SoulboundEngine {
	using System;

#nullable enable

	public static class Logger {
		private static ILoggerWrapper? wrapper;

		public static void SetWrapper(ILoggerWrapper wrapper) {
			Logger.wrapper = wrapper;
		}

		public static void LogInfo(object message) => wrapper?.LogInfo(message);
		public static void LogInfo(object message, object? context) => wrapper?.LogInfo(message, context);
		public static void LogInfo(string message, params object[] args) => wrapper?.LogInfo(message, args);

		public static void LogWarning(object message) => wrapper?.LogWarning(message);
		public static void LogWarning(object message, object? context) => wrapper?.LogWarning(message, context);
		public static void LogWarning(string message, params object[] args) => wrapper?.LogWarning(message, args);

		public static void LogError(object message) => wrapper?.LogError(message);
		public static void LogError(object message, object? context) => wrapper?.LogError(message, context);
		public static void LogError(string message, params object[] args) => wrapper?.LogError(message, args);

		public static void LogFatal(Exception? exception, object message) => wrapper?.LogFatal(exception, message);
		public static void LogFatal(Exception? exception, object message, object? context) => wrapper?.LogFatal(exception, message, context);
		public static void LogFatal(Exception? exception, string message, params object[] args) => wrapper?.LogFatal(exception, message, args);
		public static void LogFatal(Exception? exception) => wrapper?.LogFatal(exception);
	}
}
