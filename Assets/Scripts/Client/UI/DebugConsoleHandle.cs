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
		private readonly List<(GameObject obj, LogType logType)> logObjects = new();
		private readonly HashSet<LogType> filteredTypes = new();

		public GameObject LogMessageReceivedThreaded(string condition, string stackTrace, LogType logType) {
			if (logType == LogType.Error || logType == LogType.Exception) {
				int logSkips = logType == LogType.Error ? 4 : 3;
				StringBuilder builder = new();

				builder.AppendLine(condition);
				builder.Append(SkipFrames(stackTrace, logSkips));
				condition = builder.ToString();
			}

			GameObject obj = new(logType.ToString(), typeof(RectTransform));
			logObjects.Add((obj, logType));

			TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 12f;
			text.text = condition;
			text.color = logType switch {
				LogType.Log => Color.white,
				LogType.Warning => Color.yellow,
				LogType.Error => Color.red,
				LogType.Exception => Color.red,
				_ => Color.white
			};

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			return obj;
		}

		public void ToggleFilter(LogType typeFilter) {
			if (!filteredTypes.Add(typeFilter)) filteredTypes.Remove(typeFilter);

			foreach (var (obj, _) in logObjects) obj.SetActive(true);
			foreach (var (obj, _) in logObjects.Where(o => !filteredTypes.Contains(o.logType))) {
				obj.SetActive(false);
			}
		}

		private string SkipFrames(string stackTrace, int skipCount) {
			if (string.IsNullOrEmpty(stackTrace)) return stackTrace;

			IEnumerable<string> lines = stackTrace.Split("\n").Skip(skipCount);
			if (!lines.Any()) return string.Empty;

			var builder = new StringBuilder();

			foreach (var line in lines) {
				if (string.IsNullOrWhiteSpace(line)) continue;

				builder.Append('\t');
				builder.AppendLine(line.TrimStart());
			}

			return builder.ToString();
		}

		public void SetVisible(bool visible) {
			throw new NotImplementedException();
		}
	}
}
