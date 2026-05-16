using SoulboundEngine.Client.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Debug.Logging.Console {
	public sealed class LogConsole : UxmlWidget {
		private const int NEW_LOG_ENTRIES_PER_FRAME = 3;
		private readonly List<LogEntry> displayedLogs = new();
		private readonly HashSet<int> normalLogs = new();
		private readonly HashSet<int> warningLogs = new();
		private readonly HashSet<int> errorLogs = new();
		private readonly HashSet<int> fatalLogs = new();
		private readonly Queue<LogEntry> pendingLogs = new();
		private readonly object pendingLogsLock = new();
		private bool dirty = false;
		private ListView logList;

		public LogConsole() {
			Application.logMessageReceivedThreaded += (condition, stackTrace, logType) => {
				this.EnqueueLog(new LogEntry(condition, stackTrace, logType));
			};
		}

		public override void OnBind(VisualElement root) {
			base.OnBind(root);

			this.logList = root.Q<ListView>("LogList");
			this.logList.bindItem = this.OnLogAdded;
			this.logList.itemsSource = this.displayedLogs;
		}

		private void EnqueueLog(LogEntry entry) {
			lock (this.pendingLogsLock) {
				this.pendingLogs.Enqueue(entry);
				this.dirty = true;
			}
		}

		private void OnLogAdded(VisualElement element, int index) {
			LogEntry entry = this.displayedLogs[index];
			Label label = element.Q<Label>("LogLabel");
			label.text = entry.condition + this.AddStackTrace(entry);
			label.style.unityFontStyleAndWeight = FontStyle.Normal;

			switch (entry.logType) {
				case LogType.Log:
					label.style.color = Color.white;
					break;
				case LogType.Warning:
					label.style.color = Color.yellow;
					break;
				case LogType.Error:
					label.style.color = Color.red;
					break;
				case LogType.Exception:
					label.style.color = Color.red;
					label.style.unityFontStyleAndWeight = FontStyle.Bold;
					break;
				default:
					label.style.color = Color.white;
					break;
			}
		}

		private string AddStackTrace(LogEntry entry) {
			if (entry.logType is not (LogType.Error or LogType.Exception)) return "";

			int logSkips = entry.logType == LogType.Error ? 4 : 0;
			string stackTrace = this.FormatStackTrace(entry.stackTrace, logSkips);
			return string.IsNullOrEmpty(stackTrace) ? "" : $"\n{stackTrace}";
		}

		private string FormatStackTrace(string stackTrace, int skipCount) {
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

		public void Update() {
			if (!this.dirty || !this.isVisible) return;

			int remainingLogs = NEW_LOG_ENTRIES_PER_FRAME;
			lock (this.pendingLogsLock) {
				while (this.pendingLogs.Count > 0 && remainingLogs-- > 0) {
					this.AddToLogList(this.pendingLogs.Dequeue());
				}

				this.dirty = this.pendingLogs.Count > 0;
			}

			this.logList.Rebuild();
		}

		private void AddToLogList(LogEntry entry) {
			int index = this.displayedLogs.Count;
			this.displayedLogs.Add(entry);

			switch (entry.logType) {
				case LogType.Log:
					this.normalLogs.Add(index);
					break;
				case LogType.Warning:
					this.warningLogs.Add(index);
					break;
				case LogType.Error:
					this.errorLogs.Add(index);
					break;
				case LogType.Exception:
					this.fatalLogs.Add(index);
					break;
				default:
					break;
			}
		}
	}

	public sealed record LogEntry(string condition, string stackTrace, LogType logType);
}
