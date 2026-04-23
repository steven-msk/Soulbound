using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using SoulboundEngine.Core.Render.Sprite;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public readonly struct ItemRenderData {
		public readonly int stackQuantity;
		public readonly bool isStackable;
		public readonly SpriteRef spriteRef;
		public readonly Identifier? spriteAnimation;

		public ItemRenderData(string spriteKey, ItemStack itemStack)
			: this(GetSpriteFromDefaultAtlas(spriteKey), itemStack, null) {
		}

		public ItemRenderData(string spriteKey, ItemStack itemStack, Identifier? spriteAnimation)
			: this(GetSpriteFromDefaultAtlas(spriteKey), itemStack, spriteAnimation) {
		}

		public ItemRenderData(SpriteRef spriteRef, ItemStack itemStack, Identifier? spriteAnimation)
			: this(itemStack.quantity, itemStack.item.IsStackable(), spriteRef, spriteAnimation) {
		}

		public ItemRenderData(int stackQuantity, bool isStackable, SpriteRef spriteRef, Identifier? spriteAnimation) {
			this.stackQuantity = stackQuantity;
			this.isStackable = isStackable;
			this.spriteRef = spriteRef;
			this.spriteAnimation = spriteAnimation;
		}

		public static SpriteRef GetSpriteFromDefaultAtlas(string spriteKey) {
			const string defaultAtlas = "Items";

			return new SpriteRef(new AssetKey(defaultAtlas), spriteKey);
		}
	}
}
