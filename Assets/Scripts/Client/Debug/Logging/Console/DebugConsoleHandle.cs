using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.Debug.Logging.Console {
	public sealed class DebugConsoleHandle : MonoBehaviour, IDebugConsoleHandle {
		const int LOG_OBJECTS_PER_FRAME = 3;
		private ScrollRect scrollRect;
		private Transform contentParent;
		private readonly List<(GameObject obj, LogType logType)> logObjects = new();
		private readonly HashSet<LogType> filteredTypes = new();
		private readonly Queue<(string condition, string stackTrace, LogType logType)> pendingLogs = new();

		public void Init(ScrollRect scrollRect, Transform contentParent) {
			this.scrollRect = scrollRect;
			this.contentParent = contentParent;
		}

		public void AddLog(string condition, string stackTrace, LogType logType) {
			pendingLogs.Enqueue((condition, stackTrace, logType));
		}

		public void ToggleFilter(LogType typeFilter) {
			if (!filteredTypes.Add(typeFilter)) filteredTypes.Remove(typeFilter);

			foreach (var (obj, _) in logObjects) obj.SetActive(true);
			foreach (var (obj, _) in logObjects.Where(o => !filteredTypes.Contains(o.logType))) {
				obj.SetActive(false);
			}
		}

		private void Update() => ProcessLogPendings();

		private void ProcessLogPendings() {
			if (pendingLogs.Count == 0) return;

			int objLeft = LOG_OBJECTS_PER_FRAME;
			while (pendingLogs.Count > 0 && objLeft-- > 0) {
				var (condition, stackTrace, logType) = pendingLogs.Dequeue();

				GameObject obj = CreateLogObject(condition, stackTrace, logType);
				obj.transform.SetParent(contentParent, false);
				AutoScroll();
			}
		}

		private GameObject CreateLogObject(string condition, string stackTrace, LogType logType) {
			if (logType == LogType.Error || logType == LogType.Exception) {
				int logSkips = logType == LogType.Error ? 4 : 0;
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

		private void AutoScroll() {
			Canvas.ForceUpdateCanvases();
			scrollRect.verticalNormalizedPosition = 0f;
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

	}
}
