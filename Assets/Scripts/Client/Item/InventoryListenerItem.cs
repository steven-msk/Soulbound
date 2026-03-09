using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class InventoryListenerItem : Item, IInventoryListener {
		public override string name => "Inventory Listener Item";
		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public InventoryListenerItem() : base("inventoryListenerItem") {
		}

		public void OnItemAdded(Item item, Inventory inventory) {
			Logger.LogInfo("listener item added to inventory");
		}

		public void OnItemRemoved(Item item, Inventory inventory) {
			Logger.LogInfo("listener item removed from inventory");
		}
	}
}
