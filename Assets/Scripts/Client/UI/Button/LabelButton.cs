using SoulboundBackend.Client.UI;
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
	public class LabelButton : IUIElementTemplate<ButtonHandle> {
		public GameObject Instantiate() {
			GameObject obj = new("Button", typeof(RectTransform));
			UnityEngine.UI.Button button = obj.AddComponent<UnityEngine.UI.Button>();

			TextMeshProUGUI label = obj.AddComponent<TextMeshProUGUI>();
			button.targetGraphic = label;

			return obj;
		}
	}
}
