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
			node?.handle.AddLogEntry(entry);
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

		private UIDebugConsoleNode GetNode() {
			GameObject obj = new("Debug Console", typeof(RectTransform));
			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0f, 0f);
			rect.anchorMax = new Vector2(1f, 1f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = Vector2.zero;

			Image image = obj.AddComponent<Image>();
			image.color = new Color(0.15f, 0.15f, 0.15f, 0.55f);

			VerticalLayoutGroup layoutGroup = obj.AddComponent<VerticalLayoutGroup>();
			layoutGroup.childControlHeight = false;
			layoutGroup.childControlWidth = true;
			layoutGroup.childForceExpandHeight = false;
			layoutGroup.childForceExpandWidth = true;
			layoutGroup.spacing = 5f;

			DebugConsoleHandle handle = obj.AddComponent<DebugConsoleHandle>();
			foreach (var logEntry in logQueue) {
				handle.AddLogEntry(logEntry);
			}

			return new UIDebugConsoleNode(obj, handle);
		}
	}
}
