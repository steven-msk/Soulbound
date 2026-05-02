using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core.Render.Sprite;

namespace SoulboundEngine.Client.Render.Item {
	public interface IItemModelResolver {
		ItemModel Resolve(ItemStack itemStack);

		public delegate IItemModelResolver Factory(ISpriteResolver<AtlasSpriteRef> spriteResolver);

		public sealed class Default : IItemModelResolver {
			private readonly AtlasSpriteRef spriteRef;
			private readonly ISpriteResolver<AtlasSpriteRef> spriteResolver;

			public Default(ISpriteResolver<AtlasSpriteRef> spriteResolver, AtlasSpriteRef spriteRef) {
				this.spriteRef = spriteRef;
				this.spriteResolver = spriteResolver;
			}

			public ItemModel Resolve(ItemStack itemStack) {
				return new BasicItemModel(this.spriteResolver.GetSprite(this.spriteRef));
			}
		}
	}
}
