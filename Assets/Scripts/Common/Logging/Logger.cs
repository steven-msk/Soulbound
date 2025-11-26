using System;
using System.Diagnostics;
using System.Text;

#nullable enable

namespace SoulboundBackend.Common.Logging {
	public class Logger {
		const string compoundPlaceholder = "{}";

		private string caller;

		private Logger(string caller) {
			this.caller = caller;
		}

		public static Logger CreateInstance() {
			var frame = new StackFrame(1, false);
			var method = frame.GetMethod();
			var declaringType = method.DeclaringType;
			string caller = declaringType != null ? declaringType.Name : "Unknown";
			return new Logger(caller);
		}

		private void LogMessage(LogModule? module, Action<string> loggingMethod, string message) {
			StringBuilder messageBuilder = new StringBuilder();
			if (module != null) {
				messageBuilder.Append(module.Value.FormatHeaders());
				messageBuilder.Append(" ");
			}
			messageBuilder.Append($"[{caller}]: {message}");
			loggingMethod.Invoke(messageBuilder.ToString());
		}

		public void LogInfo(LogModule? module, object message) {
			this.LogMessage(module, UnityEngine.Debug.Log, message.ToString());
		}

		public void LogInfo(object message) => this.LogInfo(module: null, message);

		public void LogInfo(LogModule? module, string compoundMessage, params object[] args) {
			this.LogMessage(module, UnityEngine.Debug.Log, FormatPlaceholders(compoundMessage, compoundPlaceholder, args));
		}

		public void LogInfo(string compoundMessage, params object[] args) {
			this.LogInfo(null, compoundMessage, args);
		}

		public void LogWarning(LogModule? module, object message) {
			this.LogMessage(module, UnityEngine.Debug.LogWarning, message.ToString());
		}

		public void LogWarning(object message) => this.LogWarning(module: null, message);

		public void LogWarning(LogModule? module, string compoundMessage, params object[] args) {
			this.LogMessage(module, UnityEngine.Debug.LogWarning, FormatPlaceholders(compoundMessage, compoundPlaceholder, args));
		}

		public void LogWarning(string compoundMessage, params object[] args) {
			this.LogWarning(null, compoundMessage, args);
		}

		public void LogError(LogModule? module, object message) {
			this.LogMessage(module, UnityEngine.Debug.LogError, message.ToString());
		}

		public void LogError(object message) => this.LogError(module: null, message);

		public void LogError(LogModule? module, string compoundMessage, params object[] args) {
			this.LogMessage(module, UnityEngine.Debug.LogError, FormatPlaceholders(compoundMessage, compoundPlaceholder, args));
		}

		public void LogError(string compoundMessage, params object[] args) {
			this.LogError(null, compoundMessage, args);
		}

		public void ThrowException(LogModule? module, Exception exception, string? comment = null) {
			comment ??= "";
			this.LogMessage(module, UnityEngine.Debug.LogError, FormatPlaceholders("Exception thrown! {}: {}", compoundPlaceholder, comment, exception));
			throw exception;
		}

		private string FormatPlaceholders(string template, string placeholder, params object[] args) {
			if (string.IsNullOrEmpty(template) || args == null || args.Length == 0) {
				return template;
			}

			int argIndex = 0;
			while (template.Contains(placeholder) && argIndex < args.Length) {
				template = template.ReplaceFirst(placeholder, args[argIndex]?.ToString() ?? "null");
				argIndex++;
			}
			return template;
		}
	}
	static class StringReplaceFirst {
		public static string ReplaceFirst(this string text, string search, string replace) {
			int pos = text.IndexOf(search);
			if (pos < 0) {
				return text;
			}
			return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
		}
	}
}
