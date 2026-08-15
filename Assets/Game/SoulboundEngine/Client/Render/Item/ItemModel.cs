using UnityEngine;

namespace SoulboundEngine.Client.Render.Item {
	using Sprite = UnityEngine.Sprite;

	public class ItemModel {
		protected Sprite sprite;

		protected ItemModel(Sprite sprite) {
			this.sprite = sprite;
		}

		public Sprite GetSprite() => this.sprite;

		/// <summary>
		/// Calculates the world space scale for a given target size
		/// </summary>
		/// <param name="targetSize">The target size in unity world space</param>
		/// <returns>The world space scale relative to the target size</returns>
		public Vector2 GetScaleToWorldSize(Vector2 targetWorldSize) {
			Vector2 nativeWorldSize = this.sprite.bounds.size;
			return targetWorldSize / nativeWorldSize;
		}
	}
}
