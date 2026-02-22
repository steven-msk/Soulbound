using SoulboundBackend.Client.UI;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug {
	public sealed class DebugConsole {
		private UIDebugConsoleNode node;
		private PlayerInputActions.UIActions inputActions;
		private bool visible;
		private static readonly Queue<(string condition, string stackTrace, LogType logType)> logQueue = new();

		public DebugConsole() {
			Application.logMessageReceivedThreaded += (condition, stackTrace, logType) => {
				HandleNodeLogMessage(condition, stackTrace, logType);
				logQueue.Enqueue((condition, stackTrace, logType));
			};
			PreInitInput();
		}

		[Obsolete, PROTOTYPICAL]
		private void PreInitInput() {
			inputActions = Soulbound.instance.playerInputActions.UI;
			inputActions.Enable();
			inputActions.EnterDebugCommand.performed += _ => StartCommandInput(); 
			inputActions.EnterDebugCommand.Disable();
			inputActions.ToggleDebugConsole.performed += _ => ToggleConsole();
			inputActions.ToggleDebugConsole.Enable();
			Logger.LogInfo("Debug actions: {}", inputActions.enabled);
			Logger.LogInfo("Toggle debug console: {}", inputActions.ToggleDebugConsole.enabled);
			Logger.LogInfo("Enter debug command: {}", inputActions.EnterDebugCommand.enabled);
		}

		public void ToggleConsole() {
			visible = !visible;

			if (!visible) node.Destroy();
			else {
				node = GetNode();
				foreach (var (condition, stackTrace, logType) in logQueue) {
					HandleNodeLogMessage(condition, stackTrace, logType);
				}
				node.onDestroy += () => {
					node = null;
					inputActions.EnterDebugCommand.Disable();
				};
				Soulbound.instance.GetUIHandler().ShowOverlay(node);
				inputActions.EnterDebugCommand.Enable();
			}
		}

		private void StartCommandInput() {
			node?.handle.StartCommandInput(node.transform);
		}

		// i am NOT bothering with cleaning this up
		// dont even think about adding a scrollbar
		// (at least for now)
		private UIDebugConsoleNode GetNode() {
			GameObject root = new("Debug Console", typeof(RectTransform));
			RectTransform rect = root.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0f, 0f);
			rect.anchorMax = new Vector2(1f, 1f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = Vector2.zero;

			Image bg = root.AddComponent<Image>();
			bg.color = new Color(0.15f, 0.15f, 0.15f, 0.55f);

			ScrollRect scrollRect = root.AddComponent<ScrollRect>();
			scrollRect.horizontal = false;
			scrollRect.scrollSensitivity = 7f;

			GameObject viewport = new("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
			viewport.transform.SetParent(root.transform, false);

			RectTransform viewportRect = viewport.GetComponent<RectTransform>();
			viewportRect.anchorMin = Vector2.zero;
			viewportRect.anchorMax = Vector2.one;
			viewportRect.sizeDelta = Vector2.zero;

			viewport.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);
			viewport.GetComponent<Mask>().showMaskGraphic = false;

			GameObject content = new("Content", typeof(RectTransform));
			content.transform.SetParent(viewport.transform, false);

			RectTransform contentRect = content.GetComponent<RectTransform>();
			contentRect.anchorMin = new Vector2(0f, 1f);
			contentRect.anchorMax = new Vector2(1f, 1f);
			contentRect.pivot = new Vector2(0.5f, 1f);
			contentRect.anchoredPosition = Vector2.zero;
			contentRect.sizeDelta = Vector2.zero;

			VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
			layout.childControlHeight = true;
			layout.childControlWidth = true;
			layout.childForceExpandHeight = false;
			layout.childForceExpandWidth = true;
			layout.spacing = 5f;
			layout.padding = new RectOffset(5, 5, 5, 5);

			ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			scrollRect.viewport = viewportRect;
			scrollRect.content = contentRect;

			DebugConsoleHandle handle = root.AddComponent<DebugConsoleHandle>();

			GameObject filterContainer = new("Filters", typeof(RectTransform));
			filterContainer.transform.SetParent(root.transform, false);
			RectTransform r = filterContainer.GetComponent<RectTransform>();
			r.pivot = r.anchorMin = r.anchorMax = new Vector2(1f, 0f);
			r.anchoredPosition = Vector2.zero;
			VerticalLayoutGroup l = filterContainer.AddComponent<VerticalLayoutGroup>();
			l.childControlWidth = l.childControlHeight = false;
			l.childForceExpandWidth = l.childForceExpandHeight = false;
			ContentSizeFitter f = filterContainer.AddComponent<ContentSizeFitter>();
			f.verticalFit = f.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

			GameObject filterInfo = CreateFilterButton(LogType.Log, handle);
			filterInfo.transform.SetParent(filterContainer.transform, false);
			GameObject filterWarning = CreateFilterButton(LogType.Warning, handle);
			filterWarning.transform.SetParent(filterContainer.transform, false);
			GameObject filterError = CreateFilterButton(LogType.Error, handle);
			filterError.transform.SetParent(filterContainer.transform, false);

			return new UIDebugConsoleNode(root, handle, scrollRect, contentRect);
		}

		private GameObject CreateFilterButton(LogType logType, DebugConsoleHandle handle) {
			GameObject obj = new($"Filter {logType}", typeof(RectTransform));
			RectTransform r = obj.GetComponent<RectTransform>();
			ContentSizeFitter f = obj.AddComponent<ContentSizeFitter>();
			f.verticalFit = f.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			TextMeshProUGUI t = obj.AddComponent<TextMeshProUGUI>();
			t.text = $"Show {logType.ToString().ToUpper()}";
			t.fontSize = 20f;
			obj.AddComponent<Button>().onClick.AddListener(() => {
				handle.ToggleFilter(logType);
			});
			return obj;
		}

		private void AutoScroll(ScrollRect scrollRect) {
			Canvas.ForceUpdateCanvases();
			scrollRect.verticalNormalizedPosition = 0f;
		}

		private void HandleNodeLogMessage(string condition, string stackTrace, LogType logType) {
			GameObject logObj = node?.handle.LogMessageReceivedThreaded(condition, stackTrace, logType);
			if (logObj != null) {
				logObj.transform.SetParent(node.contentRect.transform, false);
				AutoScroll(node.scrollRect);
			}
		}
	}
}
