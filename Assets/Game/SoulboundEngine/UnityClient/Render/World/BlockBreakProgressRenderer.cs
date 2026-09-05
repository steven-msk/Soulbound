namespace SoulboundEngine.UnityClient.Render.World {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Util;
	using SoulboundEngine.World.Player;
	using UnityEngine;
	using UnityEngine.U2D;

	public class BlockBreakProgressRenderer {
		private readonly Sprite[] sprites = new Sprite[32];
		private readonly int spriteCount;
		private readonly float progressStep;
		private PlayerEntity player;
		private SpriteRenderer overlayRenderer;

		public BlockBreakProgressRenderer() {
			SpriteAtlas atlas = AssetManager.Resolve<SpriteAtlas>(new AssetKey("blockBreakOverlayAtlas"));
			this.spriteCount = atlas.GetSprites(this.sprites);
			this.progressStep = 1f / this.spriteCount;
		}

		public void SetPlayer(PlayerEntity player) {
			this.player = player;
		}

		public void Render() {
			if (this.player == null) return;
			if (this.player.GetBlockBreakPos() is not { } blockPos) {
				if (this.overlayRenderer) this.overlayRenderer.enabled = false;
				return;
			}

			if (!this.overlayRenderer) this.overlayRenderer = CreateOverlay();
			this.overlayRenderer.enabled = true;
			this.overlayRenderer.gameObject.transform.position = blockPos.GetCenter().ToVector2();

			float progress = this.player.GetBreakProgress();
			int index = Maths.FloorToInt(progress / this.progressStep);
			this.overlayRenderer.sprite = this.sprites[index];
		}

		public void Reset() {
			if (this.overlayRenderer) Object.Destroy(this.overlayRenderer.gameObject);
			this.overlayRenderer = CreateOverlay();
		}

		private static SpriteRenderer CreateOverlay() {
			GameObject prefab = AssetManager.Resolve<GameObject>(new AssetKey("blockBreakOverlay"));
			GameObject obj = Object.Instantiate(prefab);
			return obj.GetComponent<SpriteRenderer>();
		}
	}
}
