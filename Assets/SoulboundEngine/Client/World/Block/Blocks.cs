using SoulboundEngine.Core;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem {
	public partial class Blocks {
		public static readonly Block air = Registry<Block>.Add(new AirBlock());
		public static readonly Block grass = Registry<Block>.Add(new GenericBlock(new Identifier("grass"), "Grass Block", new AssetKey("grass"), 0));
		public static readonly Block dirt = Registry<Block>.Add(new GenericBlock(new Identifier("dirt"), "Dirt Block", new AssetKey("dirt"), 0));
		public static readonly Block stone = Registry<Block>.Add(new GenericBlock(new Identifier("stone"), "Stone Block", new AssetKey("stone"), 1));
		public static readonly Block wood = Registry<Block>.Add(new GenericBlock(new Identifier("wood"), "Wood", new AssetKey("wood"), 0));
		public static readonly Block leaves = Registry<Block>.Add(new LeafBlock());

		public static readonly ToggleBlock toggleBlock = Registry<Block>.Add(new ToggleBlock());
		public static readonly NeighborReactiveBlock neighborReactiveBlock = Registry<Block>.Add(new NeighborReactiveBlock());
		public static readonly TickingBlock tickingBlock = Registry<Block>.Add(new TickingBlock());
		public static readonly PulseBlock pulseBlock = Registry<Block>.Add(new PulseBlock());
		public static readonly SelfDestructBlock selfDestructBlock = Registry<Block>.Add(new SelfDestructBlock());
		public static readonly MovingTickingBlock movingTickingBlock = Registry<Block>.Add(new MovingTickingBlock());
		public static readonly AreaTriggerBlock areaTriggerBlock = Registry<Block>.Add(new AreaTriggerBlock());
	}
}
