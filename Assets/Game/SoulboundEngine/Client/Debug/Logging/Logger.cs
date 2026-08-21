namespace SoulboundEngine {
	using System;

#nullable enable

	public static partial class Logger {
		public static void LogInfo(object message, UnityEngine.Object context) {
			wrapper?.LogInfo(message, context);
		}

		public static void LogWarning(object message, UnityEngine.Object context) {
			wrapper?.LogWarning(message, context);
		}

		public static void LogError(object message, UnityEngine.Object context) {
			wrapper?.LogError(message, context);
		}

		public static void LogFatal(Exception? exception, object message, UnityEngine.Object context) {
			wrapper?.LogFatal(exception, message, context);
		}

		public static void LogFatal(Exception? exception, UnityEngine.Object context) {
			wrapper?.LogFatal(exception, context);
		}
	}
}
