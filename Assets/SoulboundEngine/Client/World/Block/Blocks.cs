using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem {
	public partial class Blocks {
		public static readonly Block air = Register(new AirBlock());
		public static readonly Block grass = Register(new GenericBlock(Identifier.Of("grass"), "Grass Block", new AssetKey("grass"), 0));
		public static readonly Block dirt = Register(new GenericBlock(Identifier.Of("dirt"), "Dirt Block", new AssetKey("dirt"), 0));
		public static readonly Block stone = Register(new GenericBlock(Identifier.Of("stone"), "Stone Block", new AssetKey("stone"), 1));
		public static readonly Block wood = Register(new GenericBlock(Identifier.Of("wood"), "Wood", new AssetKey("wood"), 0));
		public static readonly Block leaves = Register(new LeafBlock());

		public static readonly ToggleBlock toggleBlock = Register(new ToggleBlock());
		public static readonly NeighborReactiveBlock neighborReactiveBlock = Register(new NeighborReactiveBlock());
		public static readonly TickingBlock tickingBlock = Register(new TickingBlock());
		public static readonly PulseBlock pulseBlock = Register(new PulseBlock());
		public static readonly SelfDestructBlock selfDestructBlock = Register(new SelfDestructBlock());
		public static readonly MovingTickingBlock movingTickingBlock = Register(new MovingTickingBlock());
		public static readonly AreaTriggerBlock areaTriggerBlock = Register(new AreaTriggerBlock());

		private static TBlock Register<TBlock>(TBlock block) where TBlock : Block {
			return Registries.Register(Registries.BLOCKS, block.GetIdentifier(), block);
		}

		public static void Init() { }
	}
}
