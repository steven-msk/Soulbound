using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.UI.Screen;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.UI {
	public sealed class UIHandler : IInputEventHandler {
		int IInputEventHandler.priority => 1000;
		private readonly GUI gui;
		private ScreenManager screenManager;
		private UIToolkitScreenRoot screenRoot;

		public UIHandler(UIDocument uiDocument) {
			this.gui = new GUI();
			this.screenRoot = new UIToolkitScreenRoot(uiDocument);
			this.screenManager = new ScreenManager(this.screenRoot);
		}

		public void SetUIDocument(UIDocument uiDocument) {
			this.screenRoot = new UIToolkitScreenRoot(uiDocument);
			this.screenManager.Flush();
			this.screenManager = new ScreenManager(this.screenRoot);
		}

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
