using SoulboundEngine.Assets.Scripts.Client.UI.Button;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Event;
using System;
using TMPro;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Buttons {
	[PROTOTYPICAL]
	[RequireComponent(typeof(UnityEngine.UI.Button))]
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LabelButtonHandle : MonoBehaviour, IButtonHandle {
		[SerializeField] private UnityEngine.UI.Button button;
		[SerializeField] private TextMeshProUGUI label;
		private Action onClick;

		public void Build(string text, bool enabled, Action onClick, float textSize) {
			this.button = this.button != null ? this.button : this.GetComponent<UnityEngine.UI.Button>();
			this.label = this.label != null ? this.label : this.GetComponent<TextMeshProUGUI>();

			this.SetText(text);
			this.SetEnabled(enabled);
			this.SetOnClick(onClick);
			this.label.fontSize = textSize;
			this.button.onClick.AddListener(this.OnClick);
		}

		public void SetOnClick(Action action) {
			this.button.onClick.RemoveAllListeners();
			this.onClick = action;
		}

		public void SetEnabled(bool enabled) {
			this.button.interactable = enabled;
		}

		public void SetText(string text) {
			this.label.text = text;
		}

		public void SetVisible(bool visible) {
			this.gameObject.SetActive(visible);
		}

		private void OnClick() {
			this.onClick?.Invoke();
			EventBus.Publish(new ButtonClickedEvent(this));
		}
	}
}
