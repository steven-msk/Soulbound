using SoulboundEngine.Core.Render.Animation;
using SoulboundEngine.Core.Render.Sprite;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public struct ItemRenderModel {
		public SpriteRef spriteRef;
		public int stackQuantity;
		public bool showStackText;
		public SpriteAnimation? spriteAnimation;
	}
}
