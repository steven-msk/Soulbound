using System.Collections.Generic;

namespace SoulboundEngine.Client.Input {
	public class KeyBinding {
		private static readonly Dictionary<InputKey, List<KeyBinding>> BINDINGS_BY_KEY = new();
		private static readonly Dictionary<string, KeyBinding> ID_TO_BINDING = new();
		private readonly string translationKey;
		private readonly InputKey defaultKey;
		private InputKey boundKey;
		private bool isPressed;
		private int timesPressed;

		public KeyBinding(string translationKey, InputKey defaultKey) {
			this.translationKey = translationKey;
			this.boundKey = this.defaultKey = defaultKey;
			ID_TO_BINDING[translationKey] = this;
			this.SetBoundKey(defaultKey);
		}

		public string GetTranslationKey() => this.translationKey;

		public string GetBoundKeyTranslationKey() => this.boundKey.translationKey;

		public InputKey GetDefaultKey() => this.defaultKey;

		public InputKey GetBoundKey() => this.boundKey;

		public bool IsDefault() => this.defaultKey.Equals(this.boundKey);

		public bool IsUnbound() => this.boundKey.Equals(InputKey.NONE);

		public bool IsPressed() => this.isPressed;

		public bool WasPressed() {
			if (this.timesPressed <= 0) return false;
			this.timesPressed--;
			return true;
		}

		public void SetBoundKey(InputKey key) {
			InputKey previousKey = this.boundKey;
			if (BINDINGS_BY_KEY.TryGetValue(previousKey, out List<KeyBinding> bindings)) {
				bindings.Remove(this);
			}

			this.boundKey = key;

			if (!BINDINGS_BY_KEY.ContainsKey(this.boundKey)) {
				BINDINGS_BY_KEY[this.boundKey] = new List<KeyBinding>();
			}
			BINDINGS_BY_KEY[this.boundKey].Add(this);
		}

		public bool SetPressed(bool pressed) => this.isPressed = pressed;

		public static void SetKeyPressed(InputKey key, bool pressed) {
			if (BINDINGS_BY_KEY.TryGetValue(key, out List<KeyBinding> bindings)) {
				foreach (var binding in bindings) {
					binding.SetPressed(pressed);
				}
			}
		}

		public static void KeyPressed(InputKey key) {
			if (BINDINGS_BY_KEY.TryGetValue(key, out List<KeyBinding> bindings)) {
				foreach (var binding in bindings) {
					binding.timesPressed++;
				}
			}
		}

		public static KeyBinding ById(string id) => ID_TO_BINDING[id];

		internal static void Tick() {
			foreach (var binding in ID_TO_BINDING.Values) {
				binding.timesPressed = 0;
			}
		}
	}
}
