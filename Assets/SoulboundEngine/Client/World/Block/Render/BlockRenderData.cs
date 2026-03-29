using SoulboundEngine.Core.Assets;
using UnityEngine;

namespace SoulboundEngine.Client.World.BlockSystem.Render {
	public readonly struct BlockRenderData {
		public readonly AssetKey tileKey;
		public readonly Color color;

		public BlockRenderData(AssetKey tileKey) {
			this.tileKey = tileKey;
			this.color = Color.white;
		}

		public BlockRenderData(AssetKey tileKey, Color color) {
			this.tileKey = tileKey;
			this.color = color;
		}
	}
}
