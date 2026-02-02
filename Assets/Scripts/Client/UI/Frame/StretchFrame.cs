using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public sealed class StretchFrame : IUIFrame {
		private readonly Vector2 pivot;
		private readonly Vector2 offsetMin;
		private readonly Vector2 offsetMax;

		public StretchFrame(Vector2? pivot = null, Vector2? offsetMin = null, Vector2? offsetMax = null) {
			this.pivot = pivot ?? new Vector2(0.5f, 0.5f);
			this.offsetMin = offsetMin ?? Vector2.zero;
			this.offsetMax = offsetMax ?? Vector2.zero;
		}

		public void Apply(RectTransform rect, RectTransform parent) {
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = offsetMin;
			rect.offsetMax = offsetMax; 
			rect.pivot = pivot;
		}

		void IUIFrame.OnChildAdded(UIElementNode node) {
		}
	}
}
