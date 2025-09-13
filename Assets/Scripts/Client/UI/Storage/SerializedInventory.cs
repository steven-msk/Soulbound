using System.Collections.Generic;

namespace SoulboundBackend.Client.UI.Storage {
	public struct SerializedInventory {
		public int lastHotbarIndex;
		public Dictionary<string, List<SerializedItemSlot>> regions;

		public SerializedInventory(int lastHotbarIndex, Dictionary<string, List<SerializedItemSlot>> regions) {
			this.lastHotbarIndex = lastHotbarIndex;
			this.regions = regions;
		}
	}
}
