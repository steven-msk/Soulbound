using Assets.Scripts.Core.Debug.Command;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client {
	public interface IRuntimePlayerDataProvider : IEntityView {
		InventoryData GetInventory();
	}

	public struct InventoryData {
		public Dictionary<int, ItemStack?> stacks;
		public IEnumerable<int> slots;
	}
}
