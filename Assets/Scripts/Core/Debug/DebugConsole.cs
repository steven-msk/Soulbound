using SoulboundBackend.Client.UI;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug {
	public sealed class DebugConsole {
		private UIOverlayNode node;
		private bool visible;

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

		public void ToggleConsole() {
			Logger.LogInfo("toggle console");
			visible = !visible;

			if (!visible) node.Destroy();
			else {
				node = GetNode();
				node.onDestroy += () => node = null;
				Soulbound.instance.GetUIHandler().ShowOverlay(node);
			}
		}

		private UIOverlayNode GetNode() {
			Logger.LogInfo("new node");
			GameObject obj = new("Debug Console", typeof(RectTransform));
			return new UIOverlayNode(obj);
		}
	}
}
