using UnityEngine.InputSystem.Controls;

namespace SoulboundEngine.Client.SettingSystem {
	public sealed class KeybindWriter : IKeybindProcessor {
		private readonly SettingWriter writer;

		public KeybindWriter(SettingWriter writer) {
			this.writer = writer;
		}

		public KeyControl Process(KeybindEntry keybind) {
			return writer.Process(keybind);
		}
	}
}
