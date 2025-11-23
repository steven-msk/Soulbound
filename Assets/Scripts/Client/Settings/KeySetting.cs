using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
	[PROTOTYPICAL]
	public class KeySetting : SettingVisual<KeyControl> {
		[SerializeField] private TextMeshProUGUI _text;
		public TextMeshProUGUI text { get => _text; set => _text = value; }
		public KeyMapping mapping => (KeyMapping)this.settingEntry;
		private bool isRebinding;

		public override void Build() {
			_text.text = GetBindingText(settingEntry.value);
		}

		private void Update() => PollKeyboard();

		public void UpdateRebinding(KeyControl? keyControl) {
			_text.text = GetBindingText(keyControl);
		}

		public void BeginRebinding() {
			this.isRebinding = true;
			Settings.keybindMappings.BeginRebindContext(this);
		}

		private void PollKeyboard() {
			if (!isRebinding) {
				return;
			}

			foreach (var keyControl in Keyboard.current.allKeys) {
				if (keyControl.wasPressedThisFrame) {
					var appliedControl = HandleKeyPress(keyControl);

					mapping.SetValue(appliedControl);
					UpdateRebinding(appliedControl);

					this.EndRebinding();
					return;
				}
			}
		}

		private void EndRebinding() {
			this.isRebinding = false;
			Settings.keybindMappings.EndRebindContext(this);
		}

		private KeyControl? HandleKeyPress(KeyControl keyControl) {
			if (keyControl.keyCode == KeybindMappings.backtrackScreen.GetKey()) {
				return null;
			}
			return keyControl;
		}

		public string GetBindingText(KeyControl? keyControl) {
			return keyControl?.keyCode.ToString() ?? "Not Bound";
		} 
	}
}
