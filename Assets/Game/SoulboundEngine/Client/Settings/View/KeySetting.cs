using SoulboundEngine.Client.Input;
using SoulboundEngine.Common;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

#nullable enable

namespace SoulboundEngine.Client.SettingSystem.View {
	[PROTOTYPICAL]
	public class KeySetting : SettingVisual<KeyControl>, IInputContext {
		int IInputContext.priority => 100; 
		[SerializeField] private TextMeshProUGUI text;
		private bool isRebinding;

		protected override void Build() {
			SetText(GetBindingText(settingEntry.value));
		}

		private void Update() => PollRebind();

		public void BeginRebind() {
			isRebinding = true;
			SoulboundClient.Instance.InputManager.PushContext(this);
		}

		private void PollRebind() {
			if (!isRebinding) return;

			foreach (var keyControl in Keyboard.current.allKeys) {
				if (keyControl.wasPressedThisFrame) {
					KeyControl? appliedControl = keyControl.keyCode != Key.Escape
						? keyControl
						: null;

					settingEntry.SetValue(appliedControl);
					SetText(GetBindingText(appliedControl));

					EndRebind();
					return;
				}
			}
		}

		private void EndRebind() {
			isRebinding = false;
			SoulboundClient.Instance.InputManager.RemoveContext(this);
		}

		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			return isRebinding
				&& inputEvent.token.Equals(InputTokens.Keyboard.ANY)
				&& inputEvent.phase == InputActionPhase.Performed;
		}

		public string GetBindingText(KeyControl? keyControl) {
			return keyControl?.keyCode.ToString() ?? "Unbound";
		}

		public void SetText(string text) {
			this.text.text = text;
		}
	}
}
