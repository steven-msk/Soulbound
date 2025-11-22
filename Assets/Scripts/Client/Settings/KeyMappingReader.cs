using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
	public sealed class KeyMappingReader : IKeyMappingProcessor {
		private readonly SettingReader reader;

		public KeyMappingReader(SettingReader reader) {
			this.reader = reader;
		}

		public KeyControl Process(KeyMapping keyMapping) {
			reader.Decode(keyMapping, keyMapping.id, out KeyControl? value);
			return value;
		}
	}
}
