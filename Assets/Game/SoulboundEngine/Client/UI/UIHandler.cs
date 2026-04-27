using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.UI.Screens;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Screen = SoulboundEngine.Client.UI.Screens.Screen;

#nullable enable

namespace SoulboundEngine.Client.UI {
	public sealed class UIHandler : IInputContext {
		int IInputContext.priority => 1000;
		private readonly GUI gui;
		private Canvas canvas;
		private ScreenManager screenManager;
		private IScreenRoot screenRoot;
		private readonly List<UIOverlayNode> overlays = new();

		public UIHandler(Canvas initialCanvas) {
			this.gui = new GUI();
			this.canvas = initialCanvas;
			this.screenRoot = new ScreenRoot(this.canvas.transform);
			this.screenManager = new ScreenManager(this.screenRoot);
		}

		public void SetCanvas(Canvas canvas) {
			UIOverlayNode[] nodes = this.overlays.ToArray();
			for (int i = 0; i < nodes.Length; i++) nodes[i].Destroy();
			this.overlays.Clear();

			this.canvas = canvas;
			this.screenRoot = new ScreenRoot(canvas.transform);
			this.screenManager = new ScreenManager(this.screenRoot);
		}

		public Canvas GetCanvas() => this.canvas;

		public void AddOverlay(UIOverlayNode overlayNode) {
			overlayNode.gameObject.transform.SetParent(this.canvas.transform, false);
			this.overlays.Add(overlayNode);
			overlayNode.onDestroy += () => this.overlays.Remove(overlayNode);
		}

		public void SetScreen(Screen screen) => this.screenManager.PushScreen(screen);

		public void FlushScreens() => this.screenManager.Flush();

		public IScreenNavigator GetScreenNavigator() => this.screenManager;

		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			return EventSystem.current.IsPointerOverGameObject()
				&& (inputEvent.token.Equals(InputTokens.Mouse.leftClick)
					|| inputEvent.token.Equals(InputTokens.Mouse.rightClick)
					|| inputEvent.token.Equals(InputTokens.Mouse.position))
				&& inputEvent.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
		}
	}
}
