namespace SoulboundEngine.UnityClient.Render.Block {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Tilemaps;

#nullable enable

	public sealed class BlockRenderManager {
		private readonly BlockModels blockModels;

		public BlockRenderManager(List<Block> blocks) {
			this.blockModels = BlockModelRegistry.BuildModels(blocks);
		}

		public void Render(Tilemap tilemap, int x, int y, BlockState? blockState) {
			Vector3Int position = this.ToTilemapPos(x, y);

			if (blockState == null) {
				tilemap.SetTile(position, null);
				tilemap.SetColor(position, Color.white);
				return;
			}

			BlockModel model = this.blockModels.Resolve(blockState);
			tilemap.SetTile(position, model.tile);
			tilemap.SetColor(position, model.color);
		}

		public void Clear(Tilemap tilemap, int x, int y) {
			this.Render(tilemap, x, y, Blocks.AIR.DefaultState);
		}

		private Vector3Int ToTilemapPos(int x, int y) => new(x, y);
	}
}
