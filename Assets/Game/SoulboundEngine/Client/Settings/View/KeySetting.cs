using SoulboundEngine.Client.Input;
using SoulboundEngine.Common;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

#nullable enable

namespace SoulboundEngine.Client.SettingSystem.View {
	[PROTOTYPICAL]
	public class KeySetting : SettingVisual<KeyControl>, IInputEventHandler {
		int IInputEventHandler.priority => 100; 
		[SerializeField] private TextMeshProUGUI text;
		private bool isRebinding;

		protected override void Build() {
			this.SetText(this.GetBindingText(this.settingEntry.value));
		}

		private void Update() => this.PollRebind();

		public void BeginRebind() {
			this.isRebinding = true;
			SoulboundClient.Instance.InputManager.AddHandler(this);
		}

		private void PollRebind() {
			if (!this.isRebinding) return;

			foreach (var keyControl in Keyboard.current.allKeys) {
				if (keyControl.wasPressedThisFrame) {
					KeyControl? appliedControl = keyControl.keyCode != Key.Escape
						? keyControl
						: null;

					this.settingEntry.SetValue(appliedControl);
					this.SetText(this.GetBindingText(appliedControl));

					this.EndRebind();
					return;
				}
			}
		}

		private void EndRebind() {
			this.isRebinding = false;
			SoulboundClient.Instance.InputManager.RemoveHandler(this);
		}

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			yield return InputEventListener.ConsumePerformed(InputTokens.Keyboard.ANY, _ => { });
		}

		public string GetBindingText(KeyControl? keyControl) {
			return keyControl?.keyCode.ToString() ?? "Unbound";
		}

		public void SetText(string text) {
			this.text.text = text;
		}
	}
}
