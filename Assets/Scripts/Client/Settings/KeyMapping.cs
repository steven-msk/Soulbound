using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

#nullable enable

namespace SoulboundBackend.Client.Settings {
	public class KeyMapping : SettingEntry<KeyControl> {
		public event Action<InputAction?, InputAction>? onAppliedActionChanged;
		private InputAction appliedAction = null!;

		public KeyMapping(string displayName, string id, Key defaultKey, Func<Tooltip> tooltipSupplier) 
			: base(displayName, $"keyMapping.{id}", Keyboard.current[defaultKey], new KeyboardValueSet(), tooltipSupplier) {
		}

		public KeyMapping(string displayName, string id, Key defaultKey, Func<Tooltip> tooltipSupplier, Action<KeyControl, KeyControl> valueChanged) 
			: base(displayName, $"keyMapping.{id}", Keyboard.current[defaultKey], new KeyboardValueSet(), tooltipSupplier, valueChanged) {
		}

		protected virtual void ApplyBinding(InputAction inputAction) {
			inputAction.ApplyBindingOverride(0, value.path);
		}

		protected virtual void RevokeBinding(InputAction inputAction) {
			inputAction.RemoveBindingOverride(0);
		}

		public void SetAction(InputAction? action) {
			if (action == null) {
				return;
			}
			if (appliedAction != action) {
				var oldAction = appliedAction;
				oldAction.IfNotNull(RevokeBinding);
				appliedAction = action;
				onAppliedActionChanged?.Invoke(oldAction, action);
			}
			ApplyBinding(appliedAction);
		}

		public void SetKey(Key key, bool broadcastChange = true) {
			this.SetValue(Keyboard.current[key], broadcastChange);
		}
	}

	public record KeyboardValueSet : ValueSet<KeyControl> {
		public override KeyControl Decode(string value) {
			if (string.IsNullOrEmpty(value) || Keyboard.current == null) {
				return null!;
			}

			if (Enum.TryParse<Key>(value, out var key)) {
				return Keyboard.current[key];
			}
			return null!;
		}

		public override string Encode(KeyControl? value) {
			return value?.keyCode.ToString() ?? string.Empty;
		}

		public override SettingVisual<KeyControl> GetVisual(Transform parent) {
			throw new NotImplementedException();
		}

		public override bool IsValid(KeyControl value) {
			if (value == null || Keyboard.current == null) {
				return false;
			}

			return Keyboard.current.allKeys.Contains(value);
		}
	}
}
