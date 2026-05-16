using SoulboundEngine.Client.UI;
using System.Collections.Generic;
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
			label.text = entry.condition;
			label.style.unityFontStyleAndWeight = FontStyle.Normal;

			if (this.normalLogs.Contains(index)) {
				label.style.color = Color.white;
			} else if (this.warningLogs.Contains(index)) {
				label.style.color = Color.yellow;
			} else if (this.errorLogs.Contains(index)) {
				label.style.color = Color.red;
			} else if (this.fatalLogs.Contains(index)) {
				label.style.color = Color.darkRed;
				label.style.unityFontStyleAndWeight = FontStyle.Bold;
			}
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
