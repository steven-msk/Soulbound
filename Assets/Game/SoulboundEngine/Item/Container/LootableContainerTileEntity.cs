using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Loot;
using SoulboundEngine.Registry;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Player;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Item.Container {
	public abstract class LootableContainerTileEntity : TileEntity, IInventoryScreenHandlerFactory, ILootableInventory {
		public const float MIN_USABLE_DISTANCE = 5f;
		protected RegistryKey<LootTable>? lootTable;
		protected long lootTableSeed;

		protected LootableContainerTileEntity(TileEntityType tileEntityType, BlockPos blockPos, BlockState blockState)
			: base(tileEntityType, blockPos, blockState) {
		}

		public virtual bool CanPlayerUse(PlayerEntity player) {
			return Vec2d.Distance(player.GetPosition(), this.blockPos.GetCenter()) <= MIN_USABLE_DISTANCE;
		}

		protected abstract InventoryScreenHandler CreateScreenHandler(PlayerInventory playerInventory, PlayerEntity player);

		InventoryScreenHandler IInventoryScreenHandlerFactory.Create(PlayerInventory playerInventory, PlayerEntity player) {
			return this.CreateScreenHandler(playerInventory, player);
		}

		public abstract int GetSize();
		public abstract IItemSlot GetSlot(int index);
		public abstract IEnumerable<int> GetSlots();

		public long GetLootTableSeed() => this.lootTableSeed;
		public void SetLootTableSeed(long seed) => this.lootTableSeed = seed;

		public void SetLootTable(RegistryKey<LootTable>? lootTable) => this.lootTable = lootTable;
		public RegistryKey<LootTable>? GetLootTable() => this.lootTable;

		public virtual void OnOpened(PlayerEntity player) {
			if (this.lootTable != null) {
				this.GenerateLoot(player);
				this.SetLootTable(null);
			}
		}

		public virtual void OnClosed(PlayerEntity player) {
		}

		public override void WriteAdditional(JObject json) {
			base.WriteAdditional(json);
			if (this.lootTable != null) {
				json["lootTable"] = this.lootTable.value.ToString();
				json["lootTableSeed"] = this.lootTableSeed;
			}
		}

		public override void ReadAdditional(JObject json) {
			base.ReadAdditional(json);
			string lootTableKey = ((string?)json["lootTable"]) ?? string.Empty;
			long lootTableSeed = (long?)json["lootTableSeed"] ?? 0;

			if (!string.IsNullOrEmpty(lootTableKey)) {
				RegistryKey<LootTable> key = LootTables.Get(lootTableKey);
				this.SetLootTable(key, lootTableSeed);
			}
		}
	}
}
