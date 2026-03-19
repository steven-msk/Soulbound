using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Logger = SoulboundBackend.Client.Debug.Logging.Logger;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class UIHandler : IInputContext {
		int IInputContext.priority => 1000;
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
			UIOverlayNode[] nodes = overlays.ToArray();
			for (int i = 0; i < nodes.Length; i++) nodes[i].Destroy();
			overlays.Clear();

			this.canvas = canvas;
			screenManager = new ScreenManager(canvas.transform);
		}

		public Canvas GetCanvas() => canvas;

		public void AddOverlay(UIOverlayNode overlayNode) {
			overlayNode.gameObject.transform.SetParent(canvas.transform, false);
			overlays.Add(overlayNode);
			overlayNode.onDestroy += () => overlays.Remove(overlayNode);
		}

		public void SetScreen(Screen screen) =>	screenManager.PushScreen(screen);

		public void FlushScreens() => screenManager.Flush();

		public IScreenNavigator GetScreenNavigator() => screenManager;

		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			return EventSystem.current.IsPointerOverGameObject()
				&& (inputEvent.token.Equals(InputTokens.Mouse.leftClick)
					|| inputEvent.token.Equals(InputTokens.Mouse.rightClick)
					|| inputEvent.token.Equals(InputTokens.Mouse.position))
				&& inputEvent.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
		}
	}
}
