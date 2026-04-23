using SoulboundEngine.Client.UI;
using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem.Container.View {
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
