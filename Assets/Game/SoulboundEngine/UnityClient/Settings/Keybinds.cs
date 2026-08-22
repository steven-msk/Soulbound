using SoulboundEngine.UnityClient.Input;
using SoulboundEngine.Registry;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Keyboard = SoulboundEngine.UnityClient.Input.Keyboard;

#nullable enable

namespace SoulboundEngine.UnityClient.Settings {
	public sealed class Keybinds {
		private static readonly HashSet<KeyBinding> BINDINGS = new();
		public readonly KeyBinding jump = Add(new KeyBinding(ToKey("jump"), Keyboard.GetKey(Key.Space)));
		public readonly KeyBinding moveLeft = Add(new KeyBinding(ToKey("move_left"), Keyboard.GetKey(Key.A)));
		public readonly KeyBinding moveRight = Add(new KeyBinding(ToKey("move_right"), Keyboard.GetKey(Key.D)));
		public readonly KeyBinding throwItem = Add(new KeyBinding(ToKey("throw_item"), Keyboard.GetKey(Key.Q)));
		public readonly KeyBinding toggleInventory = Add(new KeyBinding(ToKey("toggle_inventory"), Keyboard.GetKey(Key.E)));

		public readonly KeyBinding toggleDebugMetrics = Add(new KeyBinding(ToKey("toggle_debug_metrics"), Keyboard.GetKey(Key.F2)));
		public readonly KeyBinding enterCommand = Add(new KeyBinding(ToKey("enter_command"), Keyboard.GetKey(Key.Slash)));
		public readonly KeyBinding toggleLogConsole = Add(new KeyBinding(ToKey("toggle_log_console"), Keyboard.GetKey(Key.F1)));

		private static KeyBinding Add(KeyBinding binding) {
			BINDINGS.Add(binding);
			return binding;
		}

		public void Process(ISettingProcessor processor) {
			foreach (var binding in BINDINGS) {
				binding.SetBoundKey(ProcessBinding(processor, binding));	
			}
		}

		private static InputKey ProcessBinding(ISettingProcessor processor, KeyBinding binding) {
			return processor.ProcessObject(binding.GetTranslationKey(), binding.GetBoundKey(), InputKey.FromTranslationKey, k => k.translationKey);
		}

		public static string ToKey(string name) {
			return Identifier.GetTranslationKey("key", name);
		}
	}
}
