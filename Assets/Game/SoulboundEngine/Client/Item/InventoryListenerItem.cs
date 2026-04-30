using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class InventoryListenerItem : Item, IContainerItemListener {
		public InventoryListenerItem(Settings settings) : base(settings) {
		}

		public void OnItemAdded(Item item, IItemContainer container) {
			if (container is not Inventory _) return;
			Logger.LogInfo("listener item added to inventory");
		}

		public void OnItemRemoved(Item item, IItemContainer container) {
			if (container is not Inventory _) return;
			Logger.LogInfo("listener item removed from inventory");
		}

	}
}
