using SoulboundBackend.Client.Input;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
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
				Soulbound.instance.GetInputManager().Rebind(token, entry);
			};
		}

		private void AddRebinds() {
			AddRebind(jump, InputTokens.Player.jump);
		}
	}
}
