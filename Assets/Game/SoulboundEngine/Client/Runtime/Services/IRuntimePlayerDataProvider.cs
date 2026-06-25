using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.World.Entity;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.Runtime.Services {
	public interface IRuntimePlayerDataProvider : IEntityView {
		InventoryData GetInventory();
	}

	public struct InventoryData {
		public Dictionary<int, ItemStack?> stacks;
		public IEnumerable<int> slots;
	}
}
