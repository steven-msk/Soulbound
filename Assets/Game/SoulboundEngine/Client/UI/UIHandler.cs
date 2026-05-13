using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.UI.Screen;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.UI {
	public sealed class UIHandler : IInputEventHandler {
		int IInputEventHandler.priority => 1000;
		private readonly GUI gui;
		private ScreenManager screenManager;
		private UIToolkitScreenRoot screenRoot;
		private readonly List<UIOverlayNode> overlays = new();

		public UIHandler(UIDocument uIDocument) {
			this.gui = new GUI();
			this.screenRoot = new UIToolkitScreenRoot(uIDocument);
			this.screenManager = new ScreenManager(this.screenRoot);
		}

		public void SetUIDocument(UIDocument uiDocument) {
			this.screenRoot = new UIToolkitScreenRoot(uiDocument);
			this.screenManager.Flush();
			this.screenManager = new ScreenManager(this.screenRoot);
		}

		[Obsolete]
		public void SetCanvas(Canvas canvas) {
			//UIOverlayNode[] nodes = this.overlays.ToArray();
			//for (int i = 0; i < nodes.Length; i++) nodes[i].Destroy();
			//this.overlays.Clear();

			//this.canvas = canvas;
			//this.screenRoot = new ScreenRoot(canvas.transform);
			//this.screenManager = new ScreenManager(this.screenRoot);
		}

		[Obsolete]
		public Canvas GetCanvas() => null;

		[Obsolete]
		public void AddOverlay(UIOverlayNode overlayNode) {
			//overlayNode.gameObject.transform.SetParent(this.canvas.transform, false);
			//this.overlays.Add(overlayNode);
			//overlayNode.onDestroy += () => this.overlays.Remove(overlayNode);
		}

		public void SetScreen(Screen.Screen screen) => this.screenManager.PushScreen(screen);

		public void FlushScreens() => this.screenManager.Flush();

		public IScreenNavigator GetScreenNavigator() => this.screenManager;

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			static InputEventListener ConsumeWhenOverGameObject(InputToken token) {
				return InputEventListener.Performed(token, _ => {
					return EventSystem.current.IsPointerOverGameObject()
						? InputHandleResult.Consume
						: InputHandleResult.Pass;
				});
			}

			return new InputEventListener[] {
				ConsumeWhenOverGameObject(InputTokens.Mouse.leftClick),
				ConsumeWhenOverGameObject(InputTokens.Mouse.rightClick),
				ConsumeWhenOverGameObject(InputTokens.Mouse.position),

				InputEventListener.ConsumePerformed(InputTokens.Keyboard.ESC, _ => {
					if (this.screenManager.GetActiveScreen()?.ReturnWithEscape ?? false) {
						this.screenManager.PopScreen();
					}
				})
			};
		}
	}
}
