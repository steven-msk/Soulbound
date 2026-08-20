using SoulboundEngine.Client.Assets;
using UnityEngine.U2D;

namespace SoulboundEngine.Client.Render.Sprite {
	using Sprite = UnityEngine.Sprite;

	public class AtlasSpriteResolver : ISpriteResolver<AtlasSpriteRef> {
		public Sprite GetSprite(AtlasSpriteRef key) {
			if (key.atlasKey == null) return null;

			SpriteAtlas atlas = AssetManager.Resolve<SpriteAtlas>(key.atlasKey);
			Sprite sprite = atlas.GetSprite(key.spriteKey);
			return sprite;
		}
	}
}
