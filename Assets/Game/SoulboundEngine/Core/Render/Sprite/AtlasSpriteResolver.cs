using SoulboundEngine.Core.Assets;
using UnityEngine.U2D;

namespace SoulboundEngine.Core.Render.Sprite {
	using Sprite = UnityEngine.Sprite;

	public class AtlasSpriteResolver : ISpriteResolver<SpriteRef> {
		public Sprite GetSprite(SpriteRef key) {
			if (key.atlasKey == null) return null;

			SpriteAtlas atlas = AssetManager.Resolve<SpriteAtlas>(key.atlasKey);
			Sprite sprite = atlas.GetSprite(key.spriteKey);
			return sprite;
		}
	}
}
