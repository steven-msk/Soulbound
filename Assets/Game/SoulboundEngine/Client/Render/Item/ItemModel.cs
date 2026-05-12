using UnityEngine;

namespace SoulboundEngine.Client.Render.Item {
	public class ItemModel {
		protected Sprite sprite;
		/// <summary>
		/// The size of this model in world space
		/// </summary>
		public readonly Vector2 size;

		protected ItemModel(Sprite sprite, Vector2 size) {
			this.sprite = sprite;
			this.size = size;
		}

		public Sprite GetSprite() => this.sprite;

		/// <summary>
		/// Calculates the world space scale for a given target size
		/// </summary>
		/// <param name="targetSize">The target size in world space</param>
		/// <returns>The world space scale relative to the target size</returns>
		public Vector2 GetScaleTo(Vector2 targetSize) => targetSize / this.size;
	}
}
