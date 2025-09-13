using SoulboundBackend.Client.ItemSystem;

namespace SoulboundBackend.Client.UI.Storage {
	public struct SerializedItemSlot {
		public int index;
		public ItemStack itemStack;

		public SerializedItemSlot(int index, ItemStack itemStack) {
			this.index = index;
			this.itemStack = itemStack;
		}
	}
}
