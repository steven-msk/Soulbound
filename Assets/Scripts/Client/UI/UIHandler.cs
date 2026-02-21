using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class UIHandler {
		private readonly GUI gui;
		private Canvas canvas;
		private ScreenManager screenManager;
		private readonly List<UIOverlayNode> overlays = new();

		public UIHandler(Canvas initialCanvas) {
			gui = new GUI();
			canvas = initialCanvas;
			screenManager = new ScreenManager(canvas.transform);
		}

		public void SetCanvas(Canvas canvas) {
			foreach (var overlay in overlays) overlay.Destroy();

			this.canvas = canvas;
			screenManager = new ScreenManager(canvas.transform);
		}

		public void ShowOverlay(UIOverlayNode overlayNode) {
			overlayNode.gameObject.transform.SetParent(canvas.transform, false);
			overlays.Add(overlayNode);
			overlayNode.onDestroy += () => overlays.Remove(overlayNode);
		}

		public void SetScreen(Screen screen) =>	screenManager.PushScreen(screen);

		public void FlushScreens() => screenManager.Flush();

		public IScreenNavigator GetScreenNavigator() => screenManager;
	}
}
