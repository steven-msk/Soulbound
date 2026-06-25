using SoulboundEngine.Client.Item.Container;

namespace SoulboundEngine.Client.Item {
	public interface IContainerItemListener {
		void OnItemAdded(Item item, IItemContainer container);
		void OnItemRemoved(Item item, IItemContainer container);
	}
}
