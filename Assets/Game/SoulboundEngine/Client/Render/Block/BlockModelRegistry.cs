using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Block {
	using Block = World.BlockSystem.Block;

	public static class BlockModelRegistry {
		private static readonly Dictionary<Block, BlockModel.IFactory> MODEL_FACTORIES = new();

		public static void Register(Block block, Func<BlockState, BlockModel> factory) {
			Register(block, BlockModel.IFactory.Of(factory));
		}

		public static void Register(Block block, BlockModel.IFactory factory) {
			MODEL_FACTORIES.Add(block, factory);
		}

		public static BlockModels BuildModels(List<Block> blocks) {
			Dictionary<BlockState, BlockModel> models = new();

			foreach (var block in blocks) {
				if (!MODEL_FACTORIES.TryGetValue(block, out BlockModel.IFactory factory)) {
					Logger.LogError("Block model factory not found: {}", Blocks.GetIdentifier(block));
					factory = BlockModel.IFactory.Of(_ => BlockModel.AIR);
				}

				foreach (var blockState in block.StateManager.GetStates()) {
					models[blockState] = factory.Create(blockState);
				}
			}

			return new BlockModels(models);
		}
	}
}
