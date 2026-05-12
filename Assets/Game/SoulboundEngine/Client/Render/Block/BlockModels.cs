using SoulboundEngine.Client.World.BlockSystem.States;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Block {
	public sealed class BlockModels {
		private readonly Dictionary<BlockState, BlockModel> modelByState = new();

		public BlockModels(Dictionary<BlockState, BlockModel> modelByState) {
			this.modelByState = modelByState;
		}

		public BlockModel Resolve(BlockState state) {
			if (state == null) return BlockModel.AIR;
			return this.modelByState.GetValueOrDefault(state, BlockModel.AIR);
		}
	}
}
