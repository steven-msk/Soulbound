using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core.Render.Sprite;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Item {
	public interface IItemModelResolver {
		ItemModel Resolve(ItemStack itemStack);

		public delegate IItemModelResolver Factory(ISpriteResolver<AtlasSpriteRef> spriteResolver);

		public sealed class Default : IItemModelResolver {
			private readonly AtlasSpriteRef spriteRef;
			private readonly ISpriteResolver<AtlasSpriteRef> spriteResolver;
			private readonly Vector2 referenceSize;

			public Default(ISpriteResolver<AtlasSpriteRef> spriteResolver, AtlasSpriteRef spriteRef, Vector2 referenceSize) {
				this.spriteRef = spriteRef;
				this.spriteResolver = spriteResolver;
				this.referenceSize = referenceSize;
			}

			public ItemModel Resolve(ItemStack itemStack) {
				return new BasicItemModel(this.spriteResolver.GetSprite(this.spriteRef), this.referenceSize);
			}
		}
	}
}
