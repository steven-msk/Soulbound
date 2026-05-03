using UnityEngine;

namespace SoulboundEngine.Client.Render.Item {
	public class ItemModel {
		protected Sprite sprite;
		public readonly Vector2 referenceSize;

		protected ItemModel(Sprite sprite, Vector2 referenceSize) {
			this.sprite = sprite;
			this.referenceSize = referenceSize;
		}

		public Sprite GetSprite() => this.sprite;

		public Vector2 GetScaleTo(Vector2 targetSize) => targetSize / this.referenceSize;
	}
}
