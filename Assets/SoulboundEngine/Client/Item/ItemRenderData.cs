using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public readonly struct ItemRenderData {
		public readonly int stackQuantity;
		public readonly bool isStackable;
		public readonly AssetKey spriteKey;

		public ItemRenderData(AssetKey spriteKey, ItemStack itemStack) {
			this.stackQuantity = itemStack.quantity;
			this.isStackable = itemStack.item.IsStackable();
			this.spriteKey = spriteKey;
		}
	}
}
