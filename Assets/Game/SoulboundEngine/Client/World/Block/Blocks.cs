using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem {
	public partial class Blocks {
		public static readonly Block air = Register("air", new AirBlock(Block.Settings.Of("Air")));
		public static readonly Block grass = Register("grass", new GenericBlock(Block.Settings.Of("Grass Block"), new AssetKey("grass")));
		public static readonly Block dirt = Register("dirt", new GenericBlock(Block.Settings.Of("Dirt Block"), new AssetKey("dirt")));
		public static readonly Block stone = Register("stone", new GenericBlock(Block.Settings.Of("Stone Block").MinBreakLevel(1), new AssetKey("stone")));
		public static readonly Block wood = Register("wood", new GenericBlock(Block.Settings.Of("Wood"), new AssetKey("wood")));
		public static readonly Block leaves = Register("leaves", new LeafBlock(Block.Settings.Of("Leaves")));

		public static readonly ToggleBlock toggleBlock = Register("toggle_block", new ToggleBlock(Block.Settings.Of("Toggle Block")));
		public static readonly NeighborReactiveBlock neighborReactiveBlock = Register("neighbor_reactive_block", new NeighborReactiveBlock(Block.Settings.Of("Neighbor Reactive Block")));
		public static readonly TickingBlock tickingBlock = Register("ticking_block", new TickingBlock(Block.Settings.Of("Ticking Block")));
		public static readonly PulseBlock pulseBlock = Register("pulse_block", new PulseBlock(Block.Settings.Of("Pulse Block")));
		public static readonly SelfDestructBlock selfDestructBlock = Register("self_destruct_block", new SelfDestructBlock(Block.Settings.Of("Self Destruct Block")));
		public static readonly MovingTickingBlock movingTickingBlock = Register("moving_ticking_block", new MovingTickingBlock(Block.Settings.Of("Moving Ticking Block")));
		public static readonly AreaTriggerBlock areaTriggerBlock = Register("area_trigger_block", new AreaTriggerBlock(Block.Settings.Of("Area Trigger Block")));

		private static TBlock Register<TBlock>(string id, TBlock block) where TBlock : Block {
			return Registries.Register(Registries.BLOCKS, Identifier.Of(id), block);
		}

		public static Identifier GetIdentifier(Block block) {
			return Registries.BLOCKS.GetIdentifier(block);
		}

		public static void Init() { }
	}
}
