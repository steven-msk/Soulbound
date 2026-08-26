namespace SoulboundEngine.UnityClient.Debug.Logging.Console {
	using SoulboundEngine.Registry;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Settings;
	using SoulboundEngine.UnityClient.UI;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using UnityEngine;
	using UnityEngine.UIElements;

	public sealed class LogConsole : UXMLWidget, IInputFocusable {
		private static readonly Identifier LOG_LIST_ELEMENT = Identifier.Of("soulbound:log_console/log_list");
		private static readonly Identifier LOG_LABEL_ELEMENT = Identifier.Of("soulbound:log_entry/log_label");
		private static readonly Identifier FILTER_INFO_ELEMENT = Identifier.Of("soulbound:log_console/info_filter");
		private static readonly Identifier FILTER_WARNING_ELEMENT = Identifier.Of("soulbound:log_console/warning_filter");
		private static readonly Identifier FILTER_ERROR_ELEMENT = Identifier.Of("soulbound:log_console/error_filter");
		private static readonly Identifier FILTER_FATAL_ELEMENT = Identifier.Of("soulbound:log_console/fatal_filter");
		private const int MAX_ENTRIES_PER_FRAME = 3;
		private const float FILTERED_ALPHA = 0.65f;
		private readonly List<DisplayedLogEntry> logs = new();
		private readonly HashSet<int> normalLogs = new();
		private readonly HashSet<int> warningLogs = new();
		private readonly HashSet<int> errorLogs = new();
		private readonly HashSet<int> fatalLogs = new();
		private readonly Queue<LogEntry> pendingLogs = new();
		private readonly object pendingLogsLock = new();
		private readonly SoulboundUnityClient client;
		private readonly Dictionary<int, HashSet<int>> filters;
		private LogFilter cachedFilter = LogFilter.ALL;
		private LogFilter filter = LogFilter.ALL;
		private bool dirty = false;
		private ListView logList;

		public LogConsole(SoulboundUnityClient client) {
			this.client = client;
			Application.logMessageReceivedThreaded += (condition, stackTrace, logType) => {
				this.EnqueueLog(new LogEntry(condition, stackTrace, logType));
			};
			if (!client.config.isRunningInEditor) {
				Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
				Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
				Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
				Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
				Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
			}
			this.filters = new Dictionary<int, HashSet<int>>() {
				[(int)LogFilter.INFO] = this.normalLogs,
				[(int)LogFilter.WARNING] = this.warningLogs,
				[(int)LogFilter.ERROR] = this.errorLogs,
				[(int)LogFilter.FATAL] = this.fatalLogs
			};
		}

		public static void CreateRoot(VisualElement parent) {
			VisualTreeAsset asset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("LogConsole"));
			asset.CloneTree(parent);
		}

		public override void OnBind(VisualElement root) {
			base.OnBind(root);

			this.logList = root.Get<ListView>(LOG_LIST_ELEMENT);
			this.logList.bindItem = this.OnLogAdded;
			this.logList.itemsSource = this.logs;

			static void SetBgColor(Button button, bool isOn) {
				Color color = button.style.backgroundColor.value;
				color.a = isOn ? 1f : FILTERED_ALPHA;
				button.style.backgroundColor = color;
			}

			Button filterInfo = root.Get<Button>(FILTER_INFO_ELEMENT);
			filterInfo.clicked += () => {
				this.ToggleFilter(LogFilter.INFO);
				SetBgColor(filterInfo, this.IsVisible(LogFilter.INFO));
			};

			Button filterWarning = root.Get<Button>(FILTER_WARNING_ELEMENT);
			filterWarning.clicked += () => {
				this.ToggleFilter(LogFilter.WARNING);
				SetBgColor(filterWarning, this.IsVisible(LogFilter.WARNING));
			};

			Button filterError = root.Get<Button>(FILTER_ERROR_ELEMENT);
			filterError.clicked += () => {
				this.ToggleFilter(LogFilter.ERROR);
				SetBgColor(filterError, this.IsVisible(LogFilter.ERROR));
			};

			Button filterFatal = root.Get<Button>(FILTER_FATAL_ELEMENT);
			filterFatal.clicked += () => {
				this.ToggleFilter(LogFilter.FATAL);
				SetBgColor(filterFatal, this.IsVisible(LogFilter.FATAL));
			};
		}

		internal void Tick() {
			if (GameSettings.keybinds.toggleLogConsole.WasPressed()) {
				if (!this.isVisible) {
					this.Show();
					this.client.PushInputFocus(this);
				} else {
					this.Hide();
					this.client.PopInputFocus(this);
				}
			}
		}

		private void EnqueueLog(LogEntry entry) {
			lock (this.pendingLogsLock) {
				this.pendingLogs.Enqueue(entry);
				this.dirty = true;
			}
		}

		private void OnLogAdded(VisualElement element, int index) {
			DisplayedLogEntry entry = this.logs[index];
			entry.element = element;
			this.logs[index] = entry;
			Label label = element.Get<Label>(LOG_LABEL_ELEMENT);
			label.text = entry.entry.condition + this.AddStackTrace(entry.entry);
			label.style.unityFontStyleAndWeight = FontStyle.Normal;

			switch (entry.entry.logType) {
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

			StringBuilder builder = new();

			foreach (string line in lines) {
				if (string.IsNullOrWhiteSpace(line)) continue;

				builder.Append('\t');
				builder.AppendLine(line.TrimStart());
			}

			return builder.ToString();
		}

		public void Update() {
			this.RebuildListIfDirty();
			this.ShowFilters();
		}

		private void RebuildListIfDirty() {
			if (!this.dirty || !this.isVisible) return;

			int remainingLogs = MAX_ENTRIES_PER_FRAME;
			lock (this.pendingLogsLock) {
				while (this.pendingLogs.Count > 0 && remainingLogs-- > 0) {
					this.AddToLogList(this.pendingLogs.Dequeue());
				}

				this.dirty = this.pendingLogs.Count > 0;
			}

			this.logList.Rebuild();
		}

		private void ShowFilters() {
			if (this.cachedFilter == this.filter) return;

			for (int i = 0; i < 4; i++) {
				int target = 1 << i;
				if (!this.filters.TryGetValue(target, out HashSet<int> logIndices)) {
					throw new ArgumentException("Missing required log filter");
				}
				bool previouslyOn = ((int)this.cachedFilter & target) != 0;
				bool currentlyOn = ((int)this.filter & target) != 0;
				if (previouslyOn == currentlyOn) continue;

				foreach (int index in logIndices) {
					DisplayedLogEntry entry = this.logs[index];
					if (entry.element == null) {
						throw new InvalidOperationException("Cannot filter logs on a missing element");
					}
					VisualElement element = entry.element;
					element.style.display = previouslyOn ? DisplayStyle.None : DisplayStyle.Flex;
				}
			}
			this.cachedFilter = this.filter;
		}

		public void ToggleFilter(LogFilter filter) {
			this.filter ^= filter;
		}

		public bool IsVisible(LogFilter filter) {
			return (this.filter & filter) != 0;
		}

		private void AddToLogList(LogEntry entry) {
			int index = this.logs.Count;
			this.logs.Add(new DisplayedLogEntry { entry = entry });

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

		bool IInputFocusable.HasKeyboardFocus() => false;

		bool IInputFocusable.IsPointerOverUI() => true;

		private struct DisplayedLogEntry {
			public LogEntry entry;
			public VisualElement element;
		}
	}

	public sealed record LogEntry(string condition, string stackTrace, LogType logType);

}
