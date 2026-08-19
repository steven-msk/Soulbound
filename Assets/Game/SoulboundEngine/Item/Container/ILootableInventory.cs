namespace SoulboundEngine.Item.Container {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Client.Debug.Logging;
	using SoulboundEngine.Loot;
	using SoulboundEngine.Loot.Context;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;

#nullable enable

	public interface ILootableInventory : IInventory {
		long GetLootTableSeed();
		void SetLootTableSeed(long seed);

		void SetLootTable(RegistryKey<LootTable>? lootTable);
		RegistryKey<LootTable>? GetLootTable();

		Level? GetLevel();
		BlockPos GetBlockPos();
	}

	public static class LootableInventoryDefaults {
		public static void SetLootTable(this ILootableInventory lootableInventory, RegistryKey<LootTable> lootTableId, long lootTableSeed) {
			lootableInventory.SetLootTable(lootTableId);
			lootableInventory.SetLootTableSeed(lootTableSeed);
		}

		public static void GenerateLoot(this ILootableInventory lootableInventory, RegistryKey<LootTable>? tableKey, PlayerEntity? player) {
			if (tableKey == null) return;

			if (!Registries.LOOT_TABLES.TryGet(tableKey, out LootTable lootTable)) {
				Logger.LogError("Could not find loot table {}", tableKey);
				return;
			}
			LootWorldContext worldContext = new(lootableInventory.GetLevel(), player?.GetLuck() ?? 0f);
			lootTable.SupplyInventory(lootableInventory, worldContext, lootableInventory.GetLootTableSeed());
		}

		public static void UnpackLootTable(this ILootableInventory lootableInventory, PlayerEntity? player) {
			RegistryKey<LootTable>? tableKey = lootableInventory.GetLootTable();
			lootableInventory.SetLootTable(null);
			lootableInventory.GenerateLoot(tableKey, player);
		}

		public static bool TryLoadLootTable(this ILootableInventory lootableInventory, JObject json) {
			string lootTableKey = ((string?)json["lootTable"]) ?? string.Empty;
			long lootTableSeed = (long?)json["lootTableSeed"] ?? 0;

			if (!string.IsNullOrEmpty(lootTableKey)) {
				RegistryKey<LootTable> key = LootTables.Get(lootTableKey);
				lootableInventory.SetLootTable(key, lootTableSeed);
				return true;
			}
			return false;
		}

		public static bool TrySaveLootTable(this ILootableInventory lootableInventory, JObject json) {
			RegistryKey<LootTable>? lootTable = lootableInventory.GetLootTable();
			if (lootTable == null) return false;

			json["lootTable"] = lootTable.value.ToString();
			json["lootTableSeed"] = lootableInventory.GetLootTableSeed();
			return true;
		}
	}
}
