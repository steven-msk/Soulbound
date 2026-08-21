namespace SoulboundEngine {
	using System;

#nullable enable

	public partial interface ILoggerWrapper {
		void LogInfo(object message, UnityEngine.Object context);

		void LogWarning(object message, UnityEngine.Object context);

		void LogError(object message, UnityEngine.Object context);

		void LogFatal(Exception? exception, object message, UnityEngine.Object context);
		void LogFatal(Exception? exception, UnityEngine.Object context);
	}
}
