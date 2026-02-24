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
		public KeybindEntry(string displayName, string id, Key defaultKey) 
			: base(displayName, $"keybind.{id}", Keyboard.current[defaultKey], new KeyboardValueSet()) {
		}

		public KeybindEntry(string displayName, string id, Key defaultKey, Action<KeyControl, KeyControl> valueChanged)
			: base(displayName, $"keybind.{id}", Keyboard.current[defaultKey], new KeyboardValueSet(), valueChanged) {
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
			if (value == "null" || Keyboard.current == null) return null!;

			return Enum.TryParse<Key>(value, out Key key)
				? Keyboard.current[key]
				: null!;
		}

		public override string Encode(KeyControl? value) {
			return value?.keyCode.ToString() ?? "null";
		}

		public override SettingVisual<KeyControl> GetVisual(Transform parent) {
			GameObject obj = new("KeySetting", typeof(RectTransform));
			obj.transform.SetParent(parent, false);

			TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 10;
			text.autoSizeTextContainer = true;
			text.textWrappingMode = TextWrappingModes.NoWrap;

			KeySetting keySetting = obj.AddComponent<KeySetting>();
			Button rebindingButton = obj.AddComponent<Button>();
			rebindingButton.onClick.AddListener(keySetting.BeginRebind);

			return keySetting;
		}

		public override bool IsValid(KeyControl? value) {
			if (Keyboard.current == null) return false;
			return Keyboard.current.allKeys.Contains(value) || value == null;
		}
	}
}
