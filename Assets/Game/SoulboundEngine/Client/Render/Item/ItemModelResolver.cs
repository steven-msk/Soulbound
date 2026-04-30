using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Core.Render.Animation;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Items {
	public class ItemModelResolver {
		public ItemRenderModel Resolve(ItemRenderData renderData) {
			SpriteAnimation? animation = null;
			if (renderData.spriteAnimation is { } identifier) {
				animation = SpriteAnimation.Registry.Get(identifier) ?? throw new KeyNotFoundException();
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
