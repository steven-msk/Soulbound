using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
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
			inputAction.ApplyBindingOverride(0, value?.path ?? "");
			logger.LogInfo(null, "applying binding: " + value ?? "null");
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

		public override void SetValue(KeyControl? value, bool broadcastChange = true) {
			var oldValue = this.value;
			if (valueSet.IsValid(value!)) {
				this.value = value!;
				if (broadcastChange) {
					base.InvokeValueChanged(oldValue, this.value!);
				}
				SetAction(this.appliedAction);
			} else {
				logger.LogError(null, "Attempted to set invalid key to mapping '{}': '{}'", id, value!.keyCode);
			}
		}
	}

	public record KeyboardValueSet : ValueSet<KeyControl> {
		public override KeyControl Decode(string value) {
			if (value == "null" || Keyboard.current == null) {
				return null!;
			}

			if (Enum.TryParse<Key>(value, out var key)) {
				return Keyboard.current[key];
			}
			return null!;
		}

		public override string Encode(KeyControl? value) {
			return value?.keyCode.ToString() ?? "null";
		}

		public override SettingVisual<KeyControl> GetVisual(Transform parent) {
			var obj = new GameObject("KeyField", typeof(RectTransform));
			obj.transform.SetParent(parent, false);

			var text = obj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 10;
			text.autoSizeTextContainer = true;
			text.textWrappingMode = TextWrappingModes.NoWrap;

			var keySetting = obj.AddComponent<KeySetting>();
			keySetting.text = text;

			var rebindingButton = obj.AddComponent<Button>();
			rebindingButton.onClick.AddListener(keySetting.BeginRebinding);

			return keySetting;
		}

		public override bool IsValid(KeyControl? value) {
			if (Keyboard.current == null) {
				return false;
			}

			return Keyboard.current.allKeys.Contains(value) || value == null;
		}
	}
}
