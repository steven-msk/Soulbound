using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Loot.Entry;
using SoulboundEngine.Client.Loot.Provider.Number;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.Loot {
	// implementation made for simplicity convenience
	public static class LootTables {
		public static readonly RegistryKey<LootTable> CHEST_TEST = Register(Identifier.Of("chest/test"));

		public static void Init() {
			Registry<LootTable>.Register(Registries.LOOT_TABLES, CHEST_TEST, LootTable.Create()
				.Pool(LootPool.Create()
					.Rolls(UniformLootNumberProvider.Create(1, 3))
					.With(ItemEntry.Create(Items.WOOD).Weight(5))
					.With(ItemEntry.Create(Items.LEAVES).Weight(3))
					.With(ItemEntry.Create(Items.DIRT).Weight(1))
				)
			.Build());
		}

		private static RegistryKey<LootTable> Register(Identifier id) {
			return RegistryKey<LootTable>.Of(Registries.LOOT_TABLES.GetKey(), id);
		}
	}
}
