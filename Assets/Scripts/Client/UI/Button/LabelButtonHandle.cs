using SoulboundBackend.Assets.Scripts.Client.UI.Button;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Event;
using System;
using TMPro;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Buttons {
	[PROTOTYPICAL]
	[RequireComponent(typeof(UnityEngine.UI.Button))]
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LabelButtonHandle : MonoBehaviour, IButtonHandle {
		[SerializeField] private UnityEngine.UI.Button button;
		[SerializeField] private TextMeshProUGUI label;
		private Action onClick;

		public void Build(string text, bool enabled, Action onClick) {
			button = button != null ? button : GetComponent<UnityEngine.UI.Button>();
			label = label != null ? label : GetComponent<TextMeshProUGUI>();

			SetText(text);
			SetEnabled(enabled);
			SetOnClick(onClick);
			button.onClick.AddListener(OnClick);
		}

		public void SetOnClick(Action action) {
			button.onClick.RemoveAllListeners();
			onClick = action;
			if (action != null) {
			}
		}

		public void SetEnabled(bool enabled) {
			button.interactable = enabled;
		}

		public void SetText(string text) {
			label.text = text;
		}

		public void SetVisible(bool visible) {
			gameObject.SetActive(visible);
		}

		private void OnClick() {
			onClick?.Invoke();
			EventBus.Publish(new ButtonClickedEvent(this));
		}
	}
}
