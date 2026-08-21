namespace SoulboundEngine {
	using System;

#nullable enable

	public partial interface ILoggerWrapper {
		void LogInfo(object message);
		void LogInfo(string message, params object[] args);

		void LogWarning(object message);
		void LogWarning(string message, params object[] args);

		void LogError(object message);
		void LogError(string message, params object[] args);

		void LogFatal(Exception? exception, object message);
		void LogFatal(Exception? exception, string message, params object[] args);
		void LogFatal(Exception? exception);
	}
}
