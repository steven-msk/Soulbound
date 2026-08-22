namespace SoulboundEngine {
	using System;

#nullable enable

	public interface ILoggerWrapper {
		void LogInfo(object message);
		void LogInfo(object message, object? context);
		void LogInfo(string message, params object[] args);

		void LogWarning(object message);
		void LogWarning(object message, object? context);
		void LogWarning(string message, params object[] args);

		void LogError(object message);
		void LogError(object message, object? context);
		void LogError(string message, params object[] args);

		void LogFatal(Exception? exception, object message);
		void LogFatal(Exception? exception, object message, object? context);
		void LogFatal(Exception? exception, string message, params object[] args);
		void LogFatal(Exception? exception);
	}
}