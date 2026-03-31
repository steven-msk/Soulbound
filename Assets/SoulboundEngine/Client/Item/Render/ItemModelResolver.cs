using SoulboundEngine.Core;
using SoulboundEngine.Core.Render.Animation;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public sealed class ItemModelResolver {
		public ItemRenderModel Resolve(ItemRenderData renderData) {
			SpriteAnimation? animation = null;
			if (renderData.spriteAnimation is { } animationKey) {
				SpriteAnimation.RegistrationKey registrationKey = new(animationKey);
				Registry<SpriteAnimation>.TryGet(registrationKey, out SpriteAnimation anim);
				animation = anim;
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
