using SoulboundBackend.Core.Debug;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public sealed class DebugConsoleHandle : MonoBehaviour, IDebugConsoleHandle {
		const string LOG_FORMAT = "[{0}] [{1}/{2}]: {3}";   // [timestamp] [stackFrame/level]: message
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
			if (entry.level == LogLevel.Fatal) text.fontStyle = FontStyles.Bold;

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}

		private string FormatMessage(LogEntry entry) {
			string message = PlaceArgs(entry.message, ARG_MARKER, entry.args);
			string timestamp = FormatTimestamp(entry.timestamp);
			string stackFrame = FormatStackFrame(entry.stackFrame);
			string level = FormatLevel(entry.level);

			string formatted = string.Format(LOG_FORMAT,
				timestamp, stackFrame, level, message
			);
			return formatted;
		}

		private string FormatTimestamp(DateTime timestamp) {
			string formatted = string.Format(TIMESTAMP_FORMAT,
				timestamp.Hour, timestamp.Minute, timestamp.Second, timestamp.Millisecond
			);
			return formatted;
		}

		private string FormatStackFrame(StackFrame stackFrame) {
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

		public void SetVisible(bool visible) {
			throw new NotImplementedException();
		}
	}
}
