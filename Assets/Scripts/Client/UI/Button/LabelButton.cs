using SoulboundBackend.Common;
using TMPro;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Buttons {
	[PROTOTYPICAL]
	public class LabelButton : IUIElementTemplate<LabelButtonHandle> {
		public GameObject Instantiate() {
			GameObject obj = new("Button", typeof(RectTransform));
			UnityEngine.UI.Button button = obj.AddComponent<UnityEngine.UI.Button>();

			TextMeshProUGUI label = obj.AddComponent<TextMeshProUGUI>();
			button.targetGraphic = label;

			return obj;
		}
	}
}
