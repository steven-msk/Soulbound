using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core.Assets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundEngine.Client.Render.Block {
	using Block = World.BlockSystem.Block;
	using Logger = Debug.Logging.Logger;

	public static class BlockModelRegistry {
		private static readonly Dictionary<Block, BlockModel.IFactory> MODEL_FACTORIES = new();

		static BlockModelRegistry() {
			Register(Blocks.AIR, _ => BlockModel.AIR);
			Register(Blocks.GRASS, _ => new BlockModel(ResolveTile("grass")));
			Register(Blocks.DIRT, _ => new BlockModel(ResolveTile("dirt")));
			Register(Blocks.STONE, _ => new BlockModel(ResolveTile("stone")));
			Register(Blocks.WOOD, _ => new BlockModel(ResolveTile("wood")));
			Register(Blocks.LEAVES, _ => new BlockModel(ResolveTile("leaves")));

			Register(Blocks.TOGGLE_BLOCK, blockState => new BlockModel(ResolveTile(
				blockState.Get(ToggleBlock.on)
					? new AssetKey("ToggleOnTile")
					: new AssetKey("ToggleOffTile")
			)));
			Register(Blocks.NEIGHBOR_REACTIVE_BLOCK, blockState => new BlockModel(ResolveTile(
				blockState.Get(NeighborReactiveBlock.on)
					? new AssetKey("ReactActiveTile")
					: new AssetKey("ReactInactiveTile")
			)));
			Register(Blocks.TICKING_BLOCK, blockState => new BlockModel(ResolveTile(
				blockState.Get(TickingBlock.on)
					? new AssetKey("TickBlockOn")
					: new AssetKey("TickBlockOff")
			)));
			Register(Blocks.PULSE_BLOCK, blockState => new BlockModel(ResolveTile(
				blockState.Get(PulseBlock.on)
					? new AssetKey("TickBlockOn")
					: new AssetKey("TickBlockOff")
			)));
			Register(Blocks.SELF_DESTRUCT_BLOCK, _ => new BlockModel(ResolveTile("RedSquareTile")));
			Register(Blocks.MOVING_TICKING_BLOCK, _ => new BlockModel(ResolveTile("WhiteSquareTile")));
			Register(Blocks.AREA_TRIGGER_BLOCK, blockState => new BlockModel(
				ResolveTile(new AssetKey("AreaTriggerTile")),
				blockState.Get(AreaTriggerBlock.inArea)
					? Color.red
					: Color.green
			));
		}

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

		[Obsolete]
		private static TileBase ResolveTile(string id) {
			return ResolveTile(new AssetKey(id));
		}

		[Obsolete]
		private static TileBase ResolveTile(AssetKey assetKey) {
			return AssetManager.Resolve<TileBase>(assetKey);
		}
	}
}
