using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public sealed class StretchFrame : IUIFrame {
		public void Apply(RectTransform rect, RectTransform parent) {
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero; 
			rect.pivot = new Vector2(0.5f, 0.5f);
		}

		public void OnChildAdded(UIElementNode node) {
		}
	}
}
