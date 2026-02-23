using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.EventSystems.EventTrigger;

namespace SoulboundBackend.Client.SettingSystem {
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
