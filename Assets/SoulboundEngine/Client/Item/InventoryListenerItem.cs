using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.ItemSystem.View;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class InventoryListenerItem : Item, IContainerItemListener {
		public override string name => "Inventory Listener Item";
		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public InventoryListenerItem() : base("inventoryListenerItem") {
		}

		public void OnItemAdded(Item item, IItemContainer container) {
			if (container is not Inventory inventory) return;
			Logger.LogInfo("listener item added to inventory");
		}

		public void OnItemRemoved(Item item, IItemContainer container) {
			if (container is not Inventory inventory) return;
			Logger.LogInfo("listener item removed from inventory");
		}
	}
}
