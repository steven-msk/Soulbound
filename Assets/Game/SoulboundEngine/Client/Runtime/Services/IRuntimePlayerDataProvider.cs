using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.EntitySystem;
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
