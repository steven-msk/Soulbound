using SoulboundBackend.Core.Debug;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class DebugConsoleHandle : MonoBehaviour, IDebugConsoleHandle {
		const string LOG_FORMAT = "[{0}] [{1}/{2}]: {3}";   // [timestamp] [callerType/level]: message
		const string TIMESTAMP_FORMAT = "{0}:{1}:{2}.{3}";  // hour:minute:second.millis
		const string ARG_MARKER = "{}";

		public void AddLogEntry(LogEntry entry) {
			CreateLog(entry);
		}

		private void CreateLog(LogEntry entry) {
			GameObject obj = new("Log Entry", typeof(RectTransform));
			obj.transform.SetParent(transform, false);

			TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 12f;
			text.text = FormatMessage(entry);
			text.color = GetLogColor(entry.level);

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}

		private string FormatMessage(LogEntry entry) {
			string message = PlaceArgs(entry.message, ARG_MARKER, entry.args);
			string timestamp = FormatTimestamp(entry.timestamp);
			string callerType = GetCallerType(entry.stackFrame);
			string level = FormatLevel(entry.level);
			message = FormatException(entry.exception, message);

			string formatted = string.Format(LOG_FORMAT,
				timestamp, callerType, level, message
			);
			if (entry.level >= LogLevel.Error) {
				formatted = $"{formatted}\n\t{FormatCall(entry.stackFrame)}";
			}
			return formatted;
		}

		private string FormatTimestamp(DateTime timestamp) {
			string formatted = string.Format(TIMESTAMP_FORMAT,
				timestamp.Hour, timestamp.Minute, timestamp.Second, timestamp.Millisecond
			);
			return formatted;
		}

		private string GetCallerType(StackFrame stackFrame) {
			var method = stackFrame.GetMethod();
			var declaringType = method.DeclaringType;
			string caller = declaringType != null ? declaringType.Name : "Unknown";
			return caller;
		}

		private string FormatLevel(LogLevel level) {
			return level.ToString().ToUpper();
		}

		private Color GetLogColor(LogLevel level) {
			return level switch {
				LogLevel.Info => new Color(0.95f, 0.95f, 0.95f, 1f),
				LogLevel.Warning => Color.yellow,
				LogLevel.Error => Color.red,
				LogLevel.Fatal => Color.red,
				_ => throw new NotImplementedException(),
			};
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

		private string FormatException(Exception? exception, string message) {
			string exceptionMark = exception != null
				? $"Exception thrown! {exception.GetType().Name}: "
				: string.Empty;
			return exceptionMark + message;
		}

		private string FormatCall(StackFrame frame) {
			if (frame == null) return string.Empty;

			MethodBase method = frame.GetMethod();
			if (method == null) return string.Empty;

			StringBuilder builder = new();
			Type declaryingType = method.DeclaringType;
			if (declaryingType != null) {
				builder.Append(declaryingType.FullName);
				builder.Append(':');
			}
			builder.Append(method.Name);

			ParameterInfo[] parameters = method.GetParameters();
			builder.Append('(');
			builder.Append(string.Join(", ", parameters.Select(p => p.ParameterType.Name)));
			builder.Append(')');

			string filePath = frame.GetFileName();
			int line = frame.GetFileLineNumber();

			if (!string.IsNullOrEmpty(filePath) && line > 0) {
				builder.Append(" (at ");
				builder.Append(GetScriptFileName(filePath));
				builder.Append(':');
				builder.Append(line);
				builder.Append(')');
			}

			return builder.ToString();
		}

		private static string GetScriptFileName(string fullPath) {
			if (string.IsNullOrEmpty(fullPath))
				return string.Empty;

			int lastSlash = fullPath.LastIndexOfAny(new[] { '/', '\\' });
			return lastSlash >= 0
				? fullPath[(lastSlash + 1)..]
				: fullPath;
		}

		public void SetVisible(bool visible) {
			throw new NotImplementedException();
		}
	}
}
