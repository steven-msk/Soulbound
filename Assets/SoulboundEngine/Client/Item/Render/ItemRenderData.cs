using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Render;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public readonly struct ItemRenderData {
		public readonly int stackQuantity;
		public readonly bool isStackable;
		public readonly SpriteRef spriteRef;

		public ItemRenderData(string spriteKey, ItemStack itemStack)
			: this(new SpriteRef(new AssetKey("Items"), spriteKey), itemStack) {
		}

		public ItemRenderData(SpriteRef spriteRef, ItemStack itemStack) {
			this.stackQuantity = itemStack.quantity;
			this.isStackable = itemStack.item.IsStackable();
			this.spriteRef = spriteRef;
		} 
	}
}
