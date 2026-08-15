using SoulboundEngine.Registry;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace SoulboundEngine.Client.Input {
	public sealed class InputKey {
		private static readonly Dictionary<string, InputKey> KEYS = new();
		public static readonly InputKey NONE = new(Identifier.GetTranslationKey("key", "none"), Key.None);
		public readonly string translationKey;
		public readonly Key keyCode;
		private bool isPressed;
		private int timesPressed;

		private InputKey(string translationKey, KeyControl keyControl)
			: this(translationKey, keyControl.keyCode) {
		}

		private InputKey(string translationKey, Key key) {
			this.translationKey = translationKey;
			this.keyCode = key;
			KEYS.Add(translationKey, this);
		}

		public static string GetTranslationKey(KeyControl control) {
			return Identifier.GetTranslationKey("key", control.name.ToLowerInvariant());
		}

		public static InputKey FromControl(KeyControl keyControl) {
			string translationKey = GetTranslationKey(keyControl);
			if (KEYS.TryGetValue(translationKey, out InputKey key)) {
				return key;
			}
			key = new InputKey(translationKey, keyControl);
			KEYS[translationKey] = key;
			return key;
		}

		/// <summary>
		/// Will throw if a key is not registered with the given translationKey
		/// </summary>
		/// <exception cref="KeyNotFoundException"></exception>
		public static InputKey FromTranslationKey(string translationKey) {
			return KEYS[translationKey];
		}

		public void SetPressed(bool pressed) => this.isPressed = pressed;

		public void OnPressed() => this.timesPressed++;

		public bool IsPressed() => this.isPressed;

		public bool WasPressed() {
			if (this.timesPressed <= 0) return false;
			this.timesPressed--;
			return true;
		}

		internal void Tick() => this.timesPressed = 0;
	}
}
