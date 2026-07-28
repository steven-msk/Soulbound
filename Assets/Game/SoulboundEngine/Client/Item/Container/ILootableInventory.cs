using SoulboundEngine.Client.Loot;
using SoulboundEngine.Client.Loot.Context;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public interface ILootableInventory : IInventory {
		long GetLootTableSeed();
		void SetLootTableSeed(long seed);

		void SetLootTable(RegistryKey<LootTable>? lootTable);
		RegistryKey<LootTable>? GetLootTable();

		Level? GetLevel();
		BlockPos GetBlockPos();
	}

	public static class LootableInventoryExtensions {
		public static void SetLootTable(this ILootableInventory lootableInventory, RegistryKey<LootTable> lootTableId, long lootTableSeed) {
			lootableInventory.SetLootTable(lootTableId);
			lootableInventory.SetLootTableSeed(lootTableSeed);
		}

		public static void GenerateLoot(this ILootableInventory lootableInventory, PlayerEntity player) {
			RegistryKey<LootTable>? tableKey = lootableInventory.GetLootTable();
			if (tableKey == null) return;

			LootTable lootTable = Registries.LOOT_TABLES.Get(tableKey).GetValue();
			LootWorldContext worldContext = new(lootableInventory.GetLevel(), player.GetLuck());
			lootTable.SupplyInventory(lootableInventory, worldContext, lootableInventory.GetLootTableSeed());
		}
	}
}
