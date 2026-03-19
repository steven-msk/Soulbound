using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.EntitySystem;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.Runtime.Services {
	public interface IRuntimePlayerDataProvider : IEntityView {
		InventoryData GetInventory();
	}

	public struct InventoryData {
		public Dictionary<int, ItemStack?> stacks;
		public IEnumerable<int> slots;
	}
}
