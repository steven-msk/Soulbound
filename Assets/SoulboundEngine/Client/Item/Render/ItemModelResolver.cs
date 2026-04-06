using SoulboundEngine.Core.Registry;
using SoulboundEngine.Core.Render.Animation;
using System;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public sealed class ItemModelResolver {
		public ItemRenderModel Resolve(ItemRenderData renderData) {
			SpriteAnimation? animation = null;
			if (renderData.spriteAnimation is { } identifier) {
				animation = Registries.SPRITE_ANIMATIONS.GetEntry(identifier)?.GetValue()
					?? throw new ArgumentException();
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
