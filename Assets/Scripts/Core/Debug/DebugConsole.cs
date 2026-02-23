using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug {
	public sealed class DebugConsole : IInputContext {
		private UIDebugConsoleNode node;
		private bool visible;
		private readonly Queue<(string condition, string stackTrace, LogType logType)> logQueue = new();

		public DebugConsole() {
			Application.logMessageReceivedThreaded += (condition, stackTrace, logType) => {
				node?.handle.AddLog(condition, stackTrace, logType);
				logQueue.Enqueue((condition, stackTrace, logType));
			};
		}

		public void ToggleConsole() {
			visible = !visible;
			CreateNodeIfNull();

			if (!visible) node.Hide();
			else node.Show();
		}

		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			if (inputEvent.token.Equals(InputTokens.Debug.toggleConsole)) {
				ToggleConsole();
				return true;
			}
			if (inputEvent.token.Equals(InputTokens.Debug.enterCommand) && visible) {
				node?.handle.StartCommandInput(node.transform);
				return true;
			}
			return false;
		}

		private void CreateNodeIfNull() {
			if (node != null) return;

			node = CreateNode();
			node.onDestroy += () => node = null;
			foreach (var (condition, stackTrace, logType) in logQueue) {
				node.handle.AddLog(condition, stackTrace, logType);
			}
			Soulbound.instance.GetUIHandler().AddOverlay(node);
		}

		// i am NOT bothering with cleaning this up
		// dont even think about adding a scrollbar
		// (at least for now)
		private UIDebugConsoleNode CreateNode() {
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
			GameObject filterExceptions = CreateFilterButton(LogType.Exception, handle);
			filterExceptions.transform.SetParent(filterContainer.transform, false);

			handle.Init(scrollRect, contentRect);

			return new UIDebugConsoleNode(root, handle);
		}

		private GameObject CreateFilterButton(LogType logType, DebugConsoleHandle handle) {
			GameObject obj = new($"Filter {logType}", typeof(RectTransform));
			RectTransform r = obj.GetComponent<RectTransform>();
			ContentSizeFitter f = obj.AddComponent<ContentSizeFitter>();
			f.verticalFit = f.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			TextMeshProUGUI t = obj.AddComponent<TextMeshProUGUI>();
			t.text = $"Show {logType.ToString().ToLower()}s";
			t.fontSize = 20f;
			obj.AddComponent<Button>().onClick.AddListener(() => {
				handle.ToggleFilter(logType);
			});
			return obj;
		}
	}
}
