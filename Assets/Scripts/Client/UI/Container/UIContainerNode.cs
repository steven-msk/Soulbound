using SoulboundBackend.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public record UIContainerNode : UIElementNode, IUIElementContainer {
		private readonly IUILayoutController layout;
		private readonly IUIFrame frame;

		public UIContainerNode(GameObject obj, IUILayoutController layout, IUIFrame frame)
			: base(obj) {
			this.layout = layout;
			this.frame = frame;

			RectTransform rect = obj.GetComponent<RectTransform>();
			RectTransform parentRect = obj.GetComponentInParent<RectTransform>();
			frame.Apply(rect, parentRect);

			layout.ApplyTo(obj);
		}

		void IUIElementContainer.AddElement(UIElementNode node) {
			node.transform.SetParent(transform, false);
			layout.OnChildAdded(node);
			frame.OnChildAdded(node);
		}
	}
}
