using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Input;
using UnityEngine.InputSystem;

#nullable enable

namespace SoulboundEngine.Client.SettingSystem {
	public sealed class Keybinds {
		public static readonly KeybindEntry backtrackScreen = new("Screen Backtrack", "screen.backtrack", Key.Escape);
		public static readonly KeybindEntry jump = new("Jump", "player.jump", Key.Space);

		public Keybinds() {
			InputSystem.EnableDevice(Keyboard.current);
			AddRebinds();

			jump.valueChanged += (_, _) => Logger.LogInfo("jump value changed");
		}

		public void ProcessMappings(IKeybindProcessor processor) {
			jump.SetValue(processor.Process(jump));
		}

		private void AddRebind(KeybindEntry entry, InputToken token) {
			entry.valueChanged += (_, _) => {
				SoulboundClient.Instance.InputManager.Rebind(token, entry);
			};
		}

		private void AddRebinds() {
			AddRebind(jump, InputTokens.Player.jump);
		}
	}
}
