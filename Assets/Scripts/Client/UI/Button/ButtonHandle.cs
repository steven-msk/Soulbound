using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Buttons {
	[PROTOTYPICAL]
	public class ButtonHandle : MonoBehaviour, IButtonHandle {
		[SerializeField] private UnityEngine.UI.Button button;
		[SerializeField] private TextMeshProUGUI label;
		private Action onClick;

		public void Build(string text, bool enabled, Action onClick) {
			button = button != null ? button : GetComponent<UnityEngine.UI.Button>();
			label = label != null ? label : GetComponent<TextMeshProUGUI>();

			SetText(text);
			SetEnabled(enabled);
			SetOnClick(onClick);
		}

		public void SetOnClick(Action action) {
			button.onClick.RemoveAllListeners();
			onClick = action;
			if (action != null) {
				button.onClick.AddListener(() => onClick());
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
	}
}
