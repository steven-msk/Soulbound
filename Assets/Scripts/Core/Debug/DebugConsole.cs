using SoulboundBackend.Client.UI;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug {
	public sealed class DebugConsole {
		private UIDebugConsoleNode node;
		private bool visible;
		private static readonly Queue<LogEntry> logQueue = new();

		// prototypical, but will be replaced once input actions are stabilized
		[PROTOTYPICAL]
		public DebugConsole() {

			// for some reason the UI input map is disabled at startup
			Soulbound.instance.playerInputActions.UI.Enable();
			Soulbound.instance.playerInputActions.UI.ToggleDebugConsole.Enable();

			Logger.LogInfo(Soulbound.instance.playerInputActions.UI.enabled);
			Logger.LogInfo(Soulbound.instance.playerInputActions.UI.ToggleDebugConsole.enabled);
			Soulbound.instance.playerInputActions.UI.ToggleDebugConsole.performed += _ => {
				ToggleConsole();
			};
		}

		public void AddLogEntry(LogEntry entry) {
			GameObject logObj = node?.handle.AddLogEntry(entry);
			if (logObj != null) {
				logObj.transform.SetParent(node.contentRect.transform, false);
				AutoScroll(node.scrollRect);
			}
			logQueue.Enqueue(entry);
		}

		public void ToggleConsole() {
			visible = !visible;

			if (!visible) node.Destroy();
			else {
				node = GetNode();
				node.onDestroy += () => node = null;
				Soulbound.instance.GetUIHandler().ShowOverlay(node);
			}
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
			foreach (var logEntry in logQueue) {
				GameObject entryObj = handle.AddLogEntry(logEntry);
				entryObj.transform.SetParent(content.transform, false);
				AutoScroll(scrollRect);
			}

			return new UIDebugConsoleNode(root, handle, scrollRect, contentRect);
		}

		private void AutoScroll(ScrollRect scrollRect) {
			Canvas.ForceUpdateCanvases();
			scrollRect.verticalNormalizedPosition = 0f;
		}
	}
}
