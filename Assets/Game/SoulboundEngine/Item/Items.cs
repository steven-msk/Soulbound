namespace SoulboundEngine.Item {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using System;

#nullable enable

	public partial class Items {
		public static readonly Item AIR = Register("air", settings => settings.StackUpTo(0));
		public static readonly Item GRASS = Register(Blocks.GRASS);
		public static readonly Item DIRT = Register(Blocks.DIRT);
		public static readonly Item STONE = Register(Blocks.STONE);
		public static readonly Item WOOD = Register(Blocks.WOOD);
		public static readonly Item LEAVES = Register(Blocks.LEAVES);
		public static readonly Item CHEST = Register(Blocks.CHEST);
		public static readonly Item WOODEN_PICKAXE = Register("wooden_pickaxe", s => s
			.Tool(new ToolSettings(ToolPower.WOOD, 100, 1.5f))
		);
		public static readonly Item STONE_PICKAXE = Register("stone_pickaxe", s => s
			.Tool(new ToolSettings(ToolPower.STONE, 250, 2f))
		);
		// TODO: implement item families (beta roadmap point)
		public static readonly Item WOODEN_HELMET = Register("wooden_helmet", s => s.Armor(ArmorType.HELMET, ArmorSettings.WOOD));
		public static readonly Item WOODEN_CHESTPLATE = Register("wooden_chestplate", s => s.Armor(ArmorType.CHESTPLATE, ArmorSettings.WOOD));
		public static readonly Item WOODEN_LEGGINGS = Register("wooden_leggings", s => s.Armor(ArmorType.LEGGINGS, ArmorSettings.WOOD));
		public static readonly Item WOODEN_BOOTS = Register("wooden_boots", s => s.Armor(ArmorType.BOOTS, ArmorSettings.WOOD));
		public static readonly Item STONE_HELMET = Register("stone_helmet", s => s.Armor(ArmorType.HELMET, ArmorSettings.STONE));
		public static readonly Item STONE_CHESTPLATE = Register("stone_chestplate", s => s.Armor(ArmorType.CHESTPLATE, ArmorSettings.STONE));
		public static readonly Item STONE_LEGGINGS = Register("stone_leggings", s => s.Armor(ArmorType.LEGGINGS, ArmorSettings.STONE));
		public static readonly Item STONE_BOOTS = Register("stone_boots", s => s.Armor(ArmorType.BOOTS, ArmorSettings.STONE));

		// TEST ITEM
		public static readonly Item JUMP_EFFECT_ITEM = Register("jump_effect_item", s => new JumpEffectItem(s), s => s
			.NonStackable()
			.Durability(10)
		);

		public static readonly Item placeableItem = Register(Blocks.MOVING_TICKING_BLOCK);
		public static readonly Item teleportPlayerItem = Register("teleport_player_item", settings => new TeleportPlayerItem(settings),
			settings => settings.NonStackable().Durability(50)
		);
		public static readonly Item debugPointer = Register("debug_pointer", settings => new DebugPointerItem(settings),
			settings => settings.NonStackable()
		);
		
		public static Item Register(string id) {
			return Register(id, Item.Create, new Item.Settings());
		}

		public static Item Register(string id, Item.Settings settings) {
			return Register(id, Item.Create, settings);
		}

		public static Item Register(string id, Func<Item.Settings, Item.Settings> settingsFactory) {
			return Register(id, settingsFactory(new Item.Settings()));
		}

		public static Item Register(string id, Func<Item.Settings, Item> factory) {
			return Register(id, factory, new Item.Settings());
		}

		public static Item Register(string id, Func<Item.Settings, Item> factory, Item.Settings settings) {
			return Register(KeyOf(id), factory, settings);
		}

		public static Item Register(string id, Func<Item.Settings, Item> factory, Func<Item.Settings, Item.Settings> settingsFactory) {
			return Register(KeyOf(id), factory, settingsFactory(new Item.Settings()));
		}

		public static Item Register(Block block) {
			return Register(block, new Item.Settings());
		}

		public static Item Register(Block block, Item.Settings settings) {
			return Register(KeyOf(Registries.BLOCKS.GetKey(block)), CreateBlockItem(block), settings);
		}

		public static Item Register(Block block, Func<Block, Item.Settings, Item> factory, Item.Settings settings) {
			return Register(KeyOf(Registries.BLOCKS.GetKey(block)), settings => factory(block, settings), settings);
		}

		public static Item Register(RegistryKey<Item> key, Func<Item.Settings, Item> factory) {
			return Register(key, factory, new Item.Settings());
		}

		public static Item Register(RegistryKey<Item> key, Func<Item.Settings, Item> factory, Item.Settings settings) {
			settings.RegistryKey(key);
			return Registry<Item>.Register(Registries.ITEMS, key, factory(settings));
		}

		private static Func<Item.Settings, Item> CreateBlockItem(Block block) {
			return settings => new BlockItem(block, settings);
		}

		private static RegistryKey<Item> KeyOf(string id) {
			return RegistryKey<Item>.Of(Registries.ITEMS.GetKey(), Identifier.Of(id));
		}

		private static RegistryKey<Item> KeyOf(RegistryKey<Block> blockKey) {
			return RegistryKey<Item>.Of(Registries.ITEMS.GetKey(), blockKey.value);
		}

		public static Identifier GetIdentifier(Item item) {
			return Registries.ITEMS.GetIdentifier(item) ?? throw new ArgumentException("Could not find item " + item.GetName());
		}

		public static RegistryEntry<Item> GetEntry(Item? item) {
			return item == null
				? GetEntry(AIR)
				: Registries.ITEMS.GetEntry(item) ?? throw new ArgumentException("Could not find item " + item.GetName());
		}

		public static RegistryEntry<Item> GetEntry(RegistryKey<Item> key) {
			return Registries.ITEMS.GetEntry(key.value) ?? throw new ArgumentException("Could not find item " + key);
		}

		public static Item? Get(Identifier id) {
			return Registries.ITEMS.Get(id);
		}

		public static void Init() { }
	}
}
