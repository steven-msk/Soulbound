namespace SoulboundEngine.Loot {
	using SoulboundEngine.Item;
	using SoulboundEngine.Loot.Entry;
	using SoulboundEngine.Loot.Provider.Number;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using System;
	using System.Collections.Generic;

#nullable enable

	public static class LootTables {
		private static readonly Dictionary<string, RegistryKey<LootTable>> KEY_BY_STRING = new();
		public static readonly RegistryKey<LootTable> CHEST_TEST = Register(Identifier.Of("chest/test"));

		// TEMP implementation made for simplicity convenience
		[Obsolete]
		public static LootTable Init(RegistryBootstrapContext context, Registry<LootTable> registry) {
			ForItem(registry, Blocks.GRASS);
			ForItem(registry, Blocks.DIRT);
			ForItem(registry, Blocks.STONE);
			ForItem(registry, Blocks.LEAVES);
			ForItem(registry, Blocks.CHEST);
			ForItem(registry, Blocks.SIGN);
			ForItem(registry, Blocks.WOOD);
			ForItem(registry, Blocks.RUBY_ORE, Items.RUBY);
			return Registry<LootTable>.Register(registry, CHEST_TEST, LootTable.Create()
				.Pool(LootPool.Create()
					.Rolls(UniformLootNumberProvider.Create(1, 3))
					.With(ItemEntry.Create(Items.WOOD).Weight(5))
					.With(ItemEntry.Create(Items.LEAVES).Weight(3))
					.With(ItemEntry.Create(Items.DIRT).Weight(1))
				)
			.Build());
		}

		private static RegistryKey<LootTable> Register(Identifier id) {
			RegistryKey<LootTable> key = RegistryKey<LootTable>.Of(Registries.LOOT_TABLES.GetKey(), id);
			KEY_BY_STRING.Add(key.value.ToString(), key);
			return key;
		}

		private static LootTable ForItem(Registry<LootTable> registry, Block block, Item? itemOverride = null) {
			RegistryKey<LootTable> key = GetKeyFromBlock(block);
			return Registry<LootTable>.Register(registry, key, LootTable.Create()
				.Pool(LootPool.Create()
					.Rolls(ConstantLootNumberProvider.Create(1))
					.With(ItemEntry.Create(itemOverride ?? block.AsItem()))
				).Build()
			);
		}

		private static RegistryKey<LootTable> GetKeyFromBlock(Block block) {
			RegistryKey<Block> blockKey = block.GetKey();
			return RegistryKey<LootTable>.Of(RegistryKeys.LOOT_TABLE, blockKey.value.WithPrefix(AbstractBlock.BLOCK_PREFIX));
		}

		public static RegistryKey<LootTable> Get(string key) => KEY_BY_STRING[key];
	}
}
