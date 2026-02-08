using SoulboundBackend.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public record UIContainerNode : UIElementNode, IUIElementContainer {
		private readonly IUIElementContainer parent;
		private readonly IUILayoutController layout;
		private readonly IUIFrame frame;

		public UIContainerNode(GameObject obj, IUIElementContainer parent, IUILayoutController layout, IUIFrame frame)
			: base(obj) {
			this.layout = layout;
			this.frame = frame;
			this.parent = parent;

			RectTransform rect = obj.GetComponent<RectTransform>();
			RectTransform parentRect = obj.GetComponentInParent<RectTransform>();
			frame.Apply(rect, parentRect);

			layout.ApplyTo(obj);
		}

		void IUIElementContainer.OnElementAdded(UIElementNode node) {
			parent.OnElementAdded(node);
		}

		void IUIElementContainer.AddElement(UIElementNode node) {
			node.transform.SetParent(transform, false);
			layout.OnChildAdded(node);
			frame.OnChildAdded(node);
			parent.OnElementAdded(node);
		}

		void IUIElementContainer.RemoveElement(UIElementNode node) {
			node.transform.SetParent(gameObject.GetComponentInParent<Transform>(), false);
			parent.OnElementRemoved(node);
		}

		void IUIElementContainer.OnElementRemoved(UIElementNode node) {
			parent.OnElementRemoved(node);
		}
	}
}
