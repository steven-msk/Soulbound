using Cysharp.Threading.Tasks;
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
		const int LOG_OBJECTS_PER_FRAME = 3;
		private readonly List<(GameObject obj, LogType logType)> logObjects = new();
		private readonly HashSet<LogType> filteredTypes = new();
		private TMP_InputField commandInput = null!;
		private bool isTypingCommand => commandInput != null;
		private readonly Queue<(string condition, string stackTrace, LogType logType)> pendingLogs = new();
		private Action<GameObject> objectAction = null!;

		public GameObject LogMessageReceivedThreaded(string condition, string stackTrace, LogType logType) {
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

		public void ToggleFilter(LogType typeFilter) {
			if (!filteredTypes.Add(typeFilter)) filteredTypes.Remove(typeFilter);

			foreach (var (obj, _) in logObjects) obj.SetActive(true);
			foreach (var (obj, _) in logObjects.Where(o => !filteredTypes.Contains(o.logType))) {
				obj.SetActive(false);
			}
		}

		void IDebugConsoleHandle.StartCommandInput(Transform parent) {
			if (isTypingCommand) return;

			CreateCommandInput(parent);
			commandInput.gameObject.SetActive(true);
			commandInput.text = "/";
			commandInput.onValueChanged.AddListener(ForceLeadingSlash);
			SetCaretToEnd();
			commandInput.ActivateInputField();
		}

		private void ForceLeadingSlash(string value) {
			commandInput.onValueChanged.RemoveListener(ForceLeadingSlash);
			commandInput.text = $"/{value}";
			SetCaretToEnd();
		}

		private void Update() => ProcessLogPendings();

		private void ProcessLogPendings() {
			if (pendingLogs.Count == 0) return;

			int objLeft = LOG_OBJECTS_PER_FRAME;
			while (pendingLogs.Count > 0 &&  objLeft-- > 0) {
				var (condition, stackTrace, logType) = pendingLogs.Dequeue();
				GameObject obj = LogMessageReceivedThreaded(condition, stackTrace, logType);
				objectAction(obj);
			}
		}

		void IDebugConsoleHandle.PendLogs(List<(string condition, string stackTrace, LogType logType)> pending, Action<GameObject> objectAction) {
			foreach (var (condition, stackTrace, logType) in pending) {
				pendingLogs.Enqueue((condition, stackTrace, logType));
			}
			this.objectAction = objectAction;
		}

		private void SetCaretToEnd() {
			int end = commandInput.text.Length;
			commandInput.caretPosition = end;
			commandInput.selectionAnchorPosition = end;
			commandInput.selectionFocusPosition = end;
		}

		private void CreateCommandInput(Transform parent) {
			GameObject obj = new("Command Input", typeof(RectTransform));
			obj.transform.SetParent(parent, false);

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = new Vector2(1f, 0f);
			rect.pivot = new Vector2(0.5f, 0f);
			rect.sizeDelta = new Vector2(0f, 30f);

			Image bg = obj.AddComponent<Image>();
			bg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

			commandInput = obj.AddComponent<TMP_InputField>();

			GameObject textObj = new("Text", typeof(RectTransform));
			textObj.transform.SetParent(obj.transform, false);

			TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 15f;
			text.color = Color.white;

			RectTransform textRect = textObj.GetComponent<RectTransform>();
			textRect.anchorMin = Vector2.zero;
			textRect.anchorMax = Vector2.one;
			textRect.offsetMin = new Vector2(10f, 0f);
			textRect.offsetMax = new Vector2(-10f, 0f);

			commandInput.textComponent = text;
			commandInput.lineType = TMP_InputField.LineType.SingleLine;
			commandInput.onSubmit.AddListener(SubmitCommand);
		}

		private void SubmitCommand(string command) {
			Core.Debug.Logging.Logger.LogInfo("submitted commmand: {}", command);
			commandInput.DeactivateInputField();
			Destroy(commandInput.gameObject);
			commandInput = null!;
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
