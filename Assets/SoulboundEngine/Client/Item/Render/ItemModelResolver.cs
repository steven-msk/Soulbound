using SoulboundEngine.Core;
using SoulboundEngine.Core.Render.Animation;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public sealed class ItemModelResolver {
		public ItemRenderModel Resolve(ItemRenderData renderData) {
			SpriteAnimation? animation = null;
			if (renderData.spriteAnimation is { } identifier) {
				animation = Registry<SpriteAnimation>.Get(identifier);
			}

			return new ItemRenderModel {
				showStackText = renderData.isStackable,
				spriteRef = renderData.spriteRef,
				stackQuantity = renderData.stackQuantity,
				spriteAnimation = animation,
			};
		}
	}
}
