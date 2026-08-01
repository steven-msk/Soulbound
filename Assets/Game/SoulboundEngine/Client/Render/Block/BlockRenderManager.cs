using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.State;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundEngine.Client.Render.Block {
	using Block = World.Block.Block;

	public sealed class BlockRenderManager {
		private readonly BlockModels blockModels;

		public BlockRenderManager(List<Block> blocks) {
			this.blockModels = BlockModelRegistry.BuildModels(blocks);
		}

		public void Render(Tilemap tilemap, BlockPos blockPos, BlockState? blockState) {
			Vector3Int position = this.ToTilemapPos(blockPos);

			if (blockState == null) {
				tilemap.SetTile(position, null);
				tilemap.SetColor(position, Color.white);
				return;
			}

			BlockModel model = this.blockModels.Resolve(blockState);
			tilemap.SetTile(position, model.tile);
			tilemap.SetColor(position, model.color);
		}

		public void Clear(Tilemap tilemap, BlockPos blockPos) {
			this.Render(tilemap, blockPos, Blocks.AIR.DefaultState);
		}

		private Vector3Int ToTilemapPos(BlockPos blockPos) => (Vector3Int)blockPos;
	}
}
