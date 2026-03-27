using SoulboundEngine.Client.ItemSystem.Container;

namespace SoulboundEngine.Client.ItemSystem {
	public interface IContainerItemListener {
		void OnItemAdded(Item item, IItemContainer container);
		void OnItemRemoved(Item item, IItemContainer container);
	}
}
