using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
	public sealed class KeybindReader : IKeybindProcessor {
		private readonly SettingReader reader;

		public KeybindReader(SettingReader reader) {
			this.reader = reader;
		}

		public KeyControl Process(KeybindEntry keybind) {
			reader.Decode(keybind, keybind.id, out KeyControl? value);
			return value;
		}
	}
}
