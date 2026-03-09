using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IInventoryListener {
		void OnItemAdded(Item item, Inventory inventory);
		void OnItemRemoved(Item item, Inventory inventory);
	}
}
