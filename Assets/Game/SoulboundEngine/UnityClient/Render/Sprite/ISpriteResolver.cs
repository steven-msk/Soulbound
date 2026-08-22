namespace SoulboundEngine.UnityClient.Render.Sprite {
	using Sprite = UnityEngine.Sprite;

	public interface ISpriteResolver<in T> {
		Sprite GetSprite(T key);
	}
}
