using SoulboundBackend.Client.UI;
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
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
	public class KeybindEntry : SettingEntry<KeyControl> {
		[Obsolete] public event Action<InputAction?, InputAction>? onAppliedActionChanged;
		[Obsolete] private InputAction? appliedAction;

		public KeybindEntry(string displayName, string id, Key defaultKey) 
			: base(displayName, $"keyMapping.{id}", Keyboard.current[defaultKey], new KeyboardValueSet()) {
		}

		public KeybindEntry(string displayName, string id, Key defaultKey, Action<KeyControl, KeyControl> valueChanged) 
			: base(displayName, $"keyMapping.{id}", Keyboard.current[defaultKey], new KeyboardValueSet(), valueChanged) {
		}

		[Obsolete]
		protected virtual void ApplyBinding(InputAction inputAction) {
			inputAction.ApplyBindingOverride(0, value?.path ?? "");
			Logger.LogInfo("applying binding: " + value ?? "null");
		}


		[Obsolete]
		protected virtual void RevokeBinding(InputAction inputAction) {
			inputAction.RemoveBindingOverride(0);
		}

		[Obsolete]
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

		public void SetValue(Key key) {
			base.SetValue(Keyboard.current[key]);
		}

		public Key GetKey() {
			return value?.keyCode ?? Key.None;
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

			//var rebindingButton = obj.AddComponent<Button>();
			//rebindingButton.onClick.AddListener(keySetting.BeginRebinding);

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
