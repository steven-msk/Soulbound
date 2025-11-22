using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
	public sealed class KeybindMappings {
		public static readonly KeyMapping jump = new("Jump", "jump", Key.Space, Tooltip.NoTooltip);

		public void ProcessMappings(IKeyMappingProcessor processor) {
			jump.SetValue(processor.Process(jump));
		}

		public void ProcessBindings(Dictionary<KeyMapping, InputAction> bindings) {
			InputAction? GetAction(KeyMapping mapping) {
				if (bindings.TryGetValue(mapping, out var action)) {
					return action;
				}
				return null;
			}

			jump.SetAction(GetAction(jump));
		}
	}
}
