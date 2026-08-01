using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.Loot;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public abstract class LootableContainerTileEntity : TileEntity, IInventoryScreenHandlerFactory, ILootableInventory {
		public const float MIN_USABLE_DISTANCE = 5f;
		protected RegistryKey<LootTable>? lootTable;
		protected long lootTableSeed;

		protected LootableContainerTileEntity(TileEntityType tileEntityType, BlockPos blockPos, BlockState blockState)
			: base(tileEntityType, blockPos, blockState) {
		}

		public virtual bool CanPlayerUse(PlayerEntity player) {
			return Vector2.Distance(player.GetPosition(), (Vector2)this.blockPos) <= MIN_USABLE_DISTANCE;
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

		public override void Write(JObject json) {
			base.Write(json);
			if (this.lootTable != null) {
				json["lootTable"] = this.lootTable.value.ToString();
				json["lootTableSeed"] = this.lootTableSeed;
			}
		}

		public override void Read(JToken json) {
			base.Read(json);
			string lootTableKey = ((string?)json["lootTable"]) ?? string.Empty;
			long lootTableSeed = (long?)json["lootTableSeed"] ?? 0;

			if (!string.IsNullOrEmpty(lootTableKey)) {
				RegistryKey<LootTable> key = LootTables.Get(lootTableKey);
				this.SetLootTable(key, lootTableSeed);
			}
		}
	}
}
