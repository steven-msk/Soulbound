using SoulboundEngine.Core.Assets;
using UnityEngine;

namespace SoulboundEngine.Client.World.BlockSystem.Render {
	public struct BlockRenderData {
		public AssetKey tileKey;
		public Color color;

		public BlockRenderData(AssetKey tileKey) {
			this.tileKey = tileKey;
			this.color = Color.white;
		}
	}
}
