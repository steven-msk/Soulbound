namespace SoulboundEngine.UnityClient.Render.World {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Util;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;
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
			Array.Sort(this.sprites, 0, this.spriteCount, Comparer<Sprite>.Create(
				(a, b) => ExtractFrameIndex(a.name).CompareTo(ExtractFrameIndex(b.name))
			));
			this.progressStep = 1f / this.spriteCount;
		}

		private static int ExtractFrameIndex(string spriteName) {
			int underscoreIndex = spriteName.LastIndexOf('_');
			string suffix = spriteName[(underscoreIndex + 1)..];

			int digitEnd = 0;
			while (digitEnd < suffix.Length && char.IsDigit(suffix[digitEnd])) {
				digitEnd++;
			}

			return int.Parse(suffix[..digitEnd]);
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
			int index = Mathf.Clamp(Maths.FloorToInt(progress / this.progressStep), 0, this.spriteCount - 1);
			this.overlayRenderer.sprite = this.sprites[index];
		}

		private static SpriteRenderer CreateOverlay() {
			GameObject prefab = AssetManager.Resolve<GameObject>(new AssetKey("blockBreakOverlay"));
			GameObject obj = UnityEngine.Object.Instantiate(prefab);
			return obj.GetComponent<SpriteRenderer>();
		}
	}
}
