using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

#nullable enable

namespace SoulboundBackend.Client.Settings {
	public sealed class KeybindMappings {
		public static readonly KeyMapping keyMapping = new("Key Mapping Test", "keyMappingTest", Key.Home, Tooltip.NoTooltip);

		public void ProcessMappings(IKeyMappingProcessor processor) {
			keyMapping.SetValue(processor.Process(keyMapping));
		}

		public void ProcessBindings(Dictionary<KeyMapping, InputAction> bindings) {
			InputAction? GetAction(KeyMapping mapping) {
				if (bindings.TryGetValue(mapping, out var action)) {
					return action;
				}
				return null;
			}

			keyMapping.SetAction(GetAction(keyMapping));
		}
	}
}
