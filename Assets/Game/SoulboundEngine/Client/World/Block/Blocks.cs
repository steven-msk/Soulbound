using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem {
	public partial class Blocks {
		public static readonly Block AIR = Register("air", new Block(Block.Settings.Of("Air")));
		public static readonly Block GRASS = Register("grass", new Block(Block.Settings.Of("Grass Block")));
		public static readonly Block DIRT = Register("dirt", new Block(Block.Settings.Of("Dirt Block")));
		public static readonly Block STONE = Register("stone", new Block(Block.Settings.Of("Stone Block").MinBreakLevel(1)));
		public static readonly Block WOOD = Register("wood", new Block(Block.Settings.Of("Wood")));
		public static readonly Block LEAVES = Register("leaves", new LeafBlock(Block.Settings.Of("Leaves")));

		public static readonly ToggleBlock TOGGLE_BLOCK = Register("toggle_block", new ToggleBlock(Block.Settings.Of("Toggle Block")));
		public static readonly NeighborReactiveBlock NEIGHBOR_REACTIVE_BLOCK = Register("neighbor_reactive_block", new NeighborReactiveBlock(Block.Settings.Of("Neighbor Reactive Block")));
		public static readonly TickingBlock TICKING_BLOCK = Register("ticking_block", new TickingBlock(Block.Settings.Of("Ticking Block")));
		public static readonly PulseBlock PULSE_BLOCK = Register("pulse_block", new PulseBlock(Block.Settings.Of("Pulse Block")));
		public static readonly SelfDestructBlock SELF_DESTRUCT_BLOCK = Register("self_destruct_block", new SelfDestructBlock(Block.Settings.Of("Self Destruct Block")));
		public static readonly MovingTickingBlock MOVING_TICKING_BLOCK = Register("moving_ticking_block", new MovingTickingBlock(Block.Settings.Of("Moving Ticking Block")));
		public static readonly AreaTriggerBlock AREA_TRIGGER_BLOCK = Register("area_trigger_block", new AreaTriggerBlock(Block.Settings.Of("Area Trigger Block")));

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
