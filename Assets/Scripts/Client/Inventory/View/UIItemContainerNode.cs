using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public record UIItemContainerNode : UIElementNode {
		public readonly IItemContainer container;
		public readonly IItemContainerHandle handle;

		public UIItemContainerNode(GameObject gameObject, IItemContainer container, IItemContainerHandle handle)
			: base(gameObject) {
			this.container = container;
			this.handle = handle;
		}
	}
}
