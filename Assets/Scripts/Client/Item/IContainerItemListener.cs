using SoulboundBackend.Client.ItemSystem.Container;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IContainerItemListener {
		void OnItemAdded(Item item, IItemContainer container);
		void OnItemRemoved(Item item, IItemContainer container);
	}
}
