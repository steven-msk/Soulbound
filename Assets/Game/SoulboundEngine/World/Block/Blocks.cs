using SoulboundEngine.Registry;
using System;

namespace SoulboundEngine.World.Block {
	public partial class Blocks {
		public static readonly Block AIR = Register("air", s => new AirBlock(s));
		public static readonly Block GRASS = Register("grass");
		public static readonly Block DIRT = Register("dirt");
		public static readonly Block STONE = Register("stone", settings => settings.MinBreakLevel(1));
		public static readonly Block WOOD = Register("wood");
		public static readonly Block LEAVES = Register("leaves", settings => new LeafBlock(settings));
		public static readonly Block CHEST = Register("chest", settings => new ChestBlock(settings));
		public static readonly Block SIGN = Register("sign", settings => new SignBlock(settings));

		// PROTOTYPICAL
		public static readonly Block TOGGLE_BLOCK = Register("toggle_block", settings => new ToggleBlock(settings));
		public static readonly Block NEIGHBOR_REACTIVE_BLOCK = Register("neighbor_reactive_block", settings => new NeighborReactiveBlock(settings));
		public static readonly Block TICKING_BLOCK = Register("ticking_block", settings => new TickingBlock(settings));
		public static readonly Block PULSE_BLOCK = Register("pulse_block", settings => new PulseBlock(settings));
		public static readonly Block SELF_DESTRUCT_BLOCK = Register("self_destruct_block", settings => new SelfDestructBlock(settings));
		public static readonly Block MOVING_TICKING_BLOCK = Register("moving_ticking_block", settings => new MovingTickingBlock(settings));

		private static Block Register(string id) {
			return Register(id, Block.Create);
		}

		private static Block Register(string id, Func<AbstractBlock.Settings, Block> factory, Func<AbstractBlock.Settings, AbstractBlock.Settings> settingsFactory) {
			return Register(id, factory, settingsFactory(new AbstractBlock.Settings()));
		}

		private static Block Register(string id, Func<AbstractBlock.Settings, AbstractBlock.Settings> settingsFactory) {
			return Register(id, settingsFactory, new AbstractBlock.Settings());
		}

		private static Block Register(string id, Func<AbstractBlock.Settings, AbstractBlock.Settings> settingsFactory, AbstractBlock.Settings settings) {
			return Register(id, Block.Create, settingsFactory(settings));
		}

		private static Block Register(string id, Func<AbstractBlock.Settings, Block> factory) {
			return Register(id, factory, new AbstractBlock.Settings());
		}

		private static Block Register(string id, Func<AbstractBlock.Settings, Block> factory, AbstractBlock.Settings settings) {
			RegistryKey<Block> key = KeyOf(id);
			settings.RegistryKey(key);
			return Registry<Block>.Register(Registries.BLOCKS, key, factory(settings));
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
