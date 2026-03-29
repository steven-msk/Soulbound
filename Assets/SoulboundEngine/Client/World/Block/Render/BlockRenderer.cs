using SoulboundEngine.Core.Assets;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundEngine.Client.World.BlockSystem.Render {
	public sealed class BlockRenderer {
		private readonly Tilemap tilemap;

		public BlockRenderer(Tilemap tilemap) {
			this.tilemap = tilemap;
		}

		public void Render(BlockRenderModel model, BlockPos blockPos) {
			AssetKey tileKey = model.tileKey;
			TileBase? tile = tileKey != null
				? AssetManager.Resolve<TileBase>(tileKey)
				: null;

			Vector3Int position = (Vector3Int)blockPos;
			tilemap.SetTile(position, tile);
			tilemap.SetColor(position, model.color);
		}
	}
}
