using SoulboundBackend.Client.UI.Tooltip;
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
	public sealed class KeybindMappings {
		private List<InputActionMap> rebindTargetMaps = new();
		private KeySetting? rebindContextOwner;

		public static readonly KeyMapping backtrackScreen = new("Screen Backtrack", "screen.backtrack", Key.Escape, Tooltip.NoTooltip);
		public static readonly KeyMapping jump = new("Jump", "jump", Key.Space, Tooltip.NoTooltip);

		public KeybindMappings() {
			InputSystem.EnableDevice(Keyboard.current);
		}

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

		public void AddRebindTargetMap(InputActionMap actionMap) {
			rebindTargetMaps.Add(actionMap);
		}

		public void RemoveRebindTargetMap(InputActionMap actionMap) {
			rebindTargetMaps.Remove(actionMap);
		}

		public void BeginRebindContext(KeySetting keySetting) {
			if (this.rebindContextOwner != null) {
				throw new InvalidOperationException("Rebind context already active");
			}
			if (keySetting == null) {
				throw new InvalidOperationException("Cannot begin rebind context with null owner");
			}
			this.rebindContextOwner = keySetting;
			this.rebindTargetMaps.ForEach(m => m.Disable());
		}

		public void EndRebindContext(KeySetting keySetting) {
			if (this.rebindContextOwner != keySetting) {
				throw new InvalidOperationException("Invalid rebind context owner");
			}
			this.rebindContextOwner = null;
			this.rebindTargetMaps.ForEach(m => m.Enable());
		}
	}
}
