using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class InventoryListenerItem : Item, IContainerItemListener {
		public override string name => "Inventory Listener Item";

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

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData(new AssetKey("bluething"), itemStack);
		}
	}
}
