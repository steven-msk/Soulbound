using SoulboundEngine.UnityClient.Debug.Logging;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace SoulboundEngine.UnityClient.Input {
	public sealed class Keyboard : InputManager.MappedInputActions {
		public const string ACTION_MAP = "Keyboard";
		private static readonly Dictionary<KeyControl, InputKey> KEY_BY_CONTROL = new();
		private static readonly Dictionary<Key, KeyControl> CONTROL_BY_CODE = new();

		public Keyboard(InputActionAsset asset)
			: base(asset) {
			foreach (var action in this.map.actions) {
				action.performed += this.KeyPressed;
				action.canceled += this.KeyReleased;

				if (action.controls.Count > 1) {
					Logger.LogWarning("Found multiple controls on keyboard action {}. Only the first one will be used.", action.name);
				}
				InputControl rawControl = action.controls[0];
				if (rawControl is not KeyControl control) {
					Logger.LogWarning("Skipping action {}: bound control '{}' is not a physical KeyControl (synthetic: {}).",
						action.name, rawControl.name, rawControl.synthetic);
					continue;
				}
				KEY_BY_CONTROL[control] = InputKey.FromControl(control);
				CONTROL_BY_CODE[control.keyCode] = control;
			}
		}

		protected override InputActionMap GetMap(InputActionAsset asset) {
			return asset.FindActionMap(ACTION_MAP, throwIfNotFound: true);
		}

		protected internal override void Tick() {
			foreach (var key in KEY_BY_CONTROL.Values) {
				key.Tick();
			}
			KeyBinding.Tick();
		}

		private void KeyPressed(InputAction.CallbackContext ctx) {
			KeyControl keyControl = (KeyControl)ctx.control;
			InputKey key = this.GetKey(keyControl);
			key.SetPressed(true);
			key.OnPressed();
			KeyBinding.SetKeyPressed(key, true);
			KeyBinding.KeyPressed(key);
		}

		private void KeyReleased(InputAction.CallbackContext ctx) {
			KeyControl keyControl = (KeyControl)ctx.control;
			InputKey key = this.GetKey(keyControl);
			key.SetPressed(false);
			KeyBinding.SetKeyPressed(key, false);
		}

		public static void ReleaseAll() {
			foreach (var key in KEY_BY_CONTROL.Values) {
				key.SetPressed(false);
			}
			KeyBinding.ReleaseAll();
		}

		public InputKey GetKey(KeyControl control) {
			return KEY_BY_CONTROL[control];
		}

		public bool IsPressed(KeyControl key) {
			return this.GetKey(key).IsPressed();
		}

		public bool WasPressed(KeyControl key) {
			return this.GetKey(key).WasPressed();
		}


		public static InputKey GetKey(Key keyCode) {
			return KEY_BY_CONTROL[CONTROL_BY_CODE[keyCode]];
		}

		public static KeyControl GetControl(Key keyCode) {
			return CONTROL_BY_CODE[keyCode];
		}
	}
}
