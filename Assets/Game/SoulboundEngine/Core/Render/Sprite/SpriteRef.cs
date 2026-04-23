using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Core.Render.Sprite {
	public readonly struct SpriteRef {
		public readonly AssetKey atlasKey;
		public readonly string spriteKey;

		public SpriteRef(AssetKey atlasKey, string spriteKey) {
			this.atlasKey = atlasKey;
			this.spriteKey = spriteKey;
		}
	}
}
