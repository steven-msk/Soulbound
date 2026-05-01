using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core.Render.Sprite;

namespace SoulboundEngine.Client.Render.Item {
	public interface IItemModelResolver {
		ItemModel Resolve(ItemStack itemStack, ISpriteResolver<SpriteRef> spriteResolver);

		public sealed class Default : IItemModelResolver {
			private readonly SpriteRef spriteRef;

			public Default(SpriteRef spriteRef) {
				this.spriteRef = spriteRef;
			}

			public ItemModel Resolve(ItemStack itemStack, ISpriteResolver<SpriteRef> spriteResolver) {
				return new BasicItemModel(spriteResolver.GetSprite(this.spriteRef));
			}
		}
	}
}
