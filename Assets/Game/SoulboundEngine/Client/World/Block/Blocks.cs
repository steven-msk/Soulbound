using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using UnityEngine;

namespace SoulboundEngine.Client.World.BlockSystem {
	public partial class Blocks {
		public static readonly Block AIR = Register("air", new Block(Block.Settings.Of("Air")));
		public static readonly Block GRASS = Register("grass", new Block(Block.Settings.Of("Grass Block").RenderFunction(new AssetKey("grass"))));
		public static readonly Block DIRT = Register("dirt", new Block(Block.Settings.Of("Dirt Block").RenderFunction(new AssetKey("dirt"))));
		public static readonly Block STONE = Register("stone", new Block(Block.Settings.Of("Stone Block").MinBreakLevel(1).RenderFunction(new AssetKey("stone"))));
		public static readonly Block WOOD = Register("wood", new Block(Block.Settings.Of("Wood").RenderFunction(new AssetKey("wood"))));
		public static readonly Block LEAVES = Register("leaves", new LeafBlock(Block.Settings.Of("Leaves").RenderFunction(new AssetKey("leaves"))));

		public static readonly ToggleBlock TOGGLE_BLOCK = Register("toggle_block", new ToggleBlock(Block.Settings.Of("Toggle Block")
			.RenderFunction(blockState => new BlockRenderData(
				blockState.Get(ToggleBlock.on)
					? new AssetKey("ToggleOnTile")
					: new AssetKey("ToggleOffTile")
			))
		));
		public static readonly NeighborReactiveBlock NEIGHBOR_REACTIVE_BLOCK = Register("neighbor_reactive_block", new NeighborReactiveBlock(Block.Settings.Of("Neighbor Reactive Block")
			.RenderFunction(blockState => new BlockRenderData(
				blockState.Get(NeighborReactiveBlock.on)
					? new AssetKey("ReactActiveTile")
					: new AssetKey("ReactInactiveTile")
			))
		));
		public static readonly TickingBlock TICKING_BLOCK = Register("ticking_block", new TickingBlock(Block.Settings.Of("Ticking Block")
			.RenderFunction(blockState => new BlockRenderData(
				blockState.Get(TickingBlock.on)
					? new AssetKey("TickBlockOn")
					: new AssetKey("TickBlockOff")
			))
		));
		public static readonly PulseBlock PULSE_BLOCK = Register("pulse_block", new PulseBlock(Block.Settings.Of("Pulse Block")
			.RenderFunction(blockState => new BlockRenderData(
				blockState.Get(PulseBlock.on)
					? new AssetKey("TickBlockOn")
					: new AssetKey("TickBlockOff")
			))
		));
		public static readonly SelfDestructBlock SELF_DESTRUCT_BLOCK = Register("self_destruct_block", new SelfDestructBlock(Block.Settings.Of("Self Destruct Block")
			.RenderFunction(new AssetKey("RedSquareTile"))
		));
		public static readonly MovingTickingBlock MOVING_TICKING_BLOCK = Register("moving_ticking_block", new MovingTickingBlock(Block.Settings.Of("Moving Ticking Block")
			.RenderFunction(new AssetKey("WhiteSquareTile"))
		));
		public static readonly AreaTriggerBlock AREA_TRIGGER_BLOCK = Register("area_trigger_block", new AreaTriggerBlock(Block.Settings.Of("Area Trigger Block")
			.RenderFunction(blockState => new BlockRenderData(
				new AssetKey("AreaTriggerTile"),
				blockState.Get(AreaTriggerBlock.inArea)
					? Color.red
					: Color.green
			))
		));

		public static TBlock Register<TBlock>(string id, TBlock block) where TBlock : Block {
			return Registry<Block>.Register(Registries.BLOCKS, KeyOf(id), block);
		}

		public static Identifier GetIdentifier(Block block) {
			return Registries.BLOCKS.GetIdentifier(block);
		}

		private static RegistryKey<Block> KeyOf(string id) {
			return RegistryKey<Block>.Of(Registries.BLOCKS.GetKey(), Identifier.Of(id));
		}

		public static void Init() { }
	}
}
