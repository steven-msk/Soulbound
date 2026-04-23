using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem {
	public partial class Blocks {
		public static readonly Block air = Register("air", new AirBlock());
		public static readonly Block grass = Register("grass", new GenericBlock("Grass Block", new AssetKey("grass"), 0));
		public static readonly Block dirt = Register("dirt", new GenericBlock("Dirt Block", new AssetKey("dirt"), 0));
		public static readonly Block stone = Register("stone", new GenericBlock("Stone Block", new AssetKey("stone"), 1));
		public static readonly Block wood = Register("wood", new GenericBlock("Wood", new AssetKey("wood"), 0));
		public static readonly Block leaves = Register("leaves", new LeafBlock());

		public static readonly ToggleBlock toggleBlock = Register("toggle_block", new ToggleBlock());
		public static readonly NeighborReactiveBlock neighborReactiveBlock = Register("neighbor_reactive_block", new NeighborReactiveBlock());
		public static readonly TickingBlock tickingBlock = Register("ticking_block", new TickingBlock());
		public static readonly PulseBlock pulseBlock = Register("pulse_block", new PulseBlock());
		public static readonly SelfDestructBlock selfDestructBlock = Register("self_destruct_block", new SelfDestructBlock());
		public static readonly MovingTickingBlock movingTickingBlock = Register("moving_ticking_block", new MovingTickingBlock());
		public static readonly AreaTriggerBlock areaTriggerBlock = Register("area_trigger_block", new AreaTriggerBlock());

		private static TBlock Register<TBlock>(string id, TBlock block) where TBlock : Block {
			return Registries.Register(Registries.BLOCKS, Identifier.Of(id), block);
		}

		public static Identifier GetIdentifier(Block block) {
			return Registries.BLOCKS.GetIdentifier(block);
		}

		public static void Init() { }
	}
}
