using SoulboundBackend.Client.UI;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core.Debug {
	public sealed class DebugConsole {
		private UIOverlayNode node;
		private bool visible;

		public DebugConsole() {

			// prototypical, but will be replaced once input actions are stabilized
			Soulbound.instance.playerInputActions.UI.ToggleDebugConsole.performed += _ => {
				ToggleConsole();
			};
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

		private static UIOverlayNode GetNode() {
			GameObject obj = new("Debug Console", typeof(RectTransform));
			return new UIOverlayNode(obj);
		}
	}
}
