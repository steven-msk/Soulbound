using UnityEngine;

namespace SoulboundEngine.Core.Render {
	public interface ISpriteResolver<in T> {
		Sprite GetSprite(T key);
	}
}
