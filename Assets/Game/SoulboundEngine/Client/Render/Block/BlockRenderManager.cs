using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundEngine.Client.Render.Block {
	using Block = World.BlockSystem.Block;

	public sealed class BlockRenderManager {
		private readonly BlockModels blockModels;

		public BlockRenderManager(List<Block> blocks) {
			this.blockModels = BlockModelRegistry.BuildModels(blocks);
		}

		public void Render(Tilemap tilemap, BlockPos blockPos, BlockState blockState) {
			BlockModel model = this.blockModels.Resolve(blockState);
			Vector3Int position = this.ToTilemapPos(blockPos);

			tilemap.SetTile(position, model.tile);
			tilemap.SetColor(position, model.color);
		}

		public void Clear(Tilemap tilemap, BlockPos blockPos) {
			this.Render(tilemap, blockPos, Blocks.AIR.DefaultState);
		}

		private Vector3Int ToTilemapPos(BlockPos blockPos) => (Vector3Int)blockPos;
	}
}
