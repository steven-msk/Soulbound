using SoulboundEngine.Client.Assets;

namespace SoulboundEngine.Client.Render.Sprite {
	public readonly struct AtlasSpriteRef {
		public readonly AssetKey atlasKey;
		public readonly string spriteKey;

		public AtlasSpriteRef(AssetKey atlasKey, string spriteKey) {
			this.atlasKey = atlasKey;
			this.spriteKey = spriteKey;
		}
	}
}
