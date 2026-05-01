using UnityEngine;

namespace SoulboundEngine.Client.Render.Item {
	public class ItemModel {
		protected Sprite sprite;

		protected ItemModel(Sprite sprite) {
			this.sprite = sprite;
		}

		public Sprite GetSprite() => this.sprite;
	}
}
