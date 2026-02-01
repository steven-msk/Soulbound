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

		public UIContainerNode(GameObject obj, IUILayoutController layout)
			: base(obj) {
			this.layout = layout;
			layout.ApplyTo(obj);
		}

		void IUIElementContainer.AddElement(UIElementNode node) {
			node.transform.SetParent(transform, false);
			layout.OnChildAdded(node);	
		}
	}
}
