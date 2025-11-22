using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.EventSystems.EventTrigger;

namespace SoulboundBackend.Client.SettingSystem {
	public sealed class KeyMappingWriter : IKeyMappingProcessor {
		private readonly SettingWriter writer;

		public KeyMappingWriter(SettingWriter writer) {
			this.writer = writer;
		}

		public KeyControl Process(KeyMapping keyMapping) {
			return writer.Process(keyMapping);
		}
	}
}
