using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public class WorldItemView : MonoBehaviour {
		private SpriteRenderer spriteRenderer;

		public void Init(SpriteRenderer spriteRenderer) {
			this.spriteRenderer = spriteRenderer;
		}

		public SpriteRenderer GetSpriteRenderer() => spriteRenderer;
	}
}
