namespace SoulboundEngine.Client.Render.Sprite {
	using Sprite = UnityEngine.Sprite;

	public interface ISpriteResolver<in T> {
		Sprite GetSprite(T key);
	}
}
