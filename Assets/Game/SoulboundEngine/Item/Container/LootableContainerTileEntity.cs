namespace SoulboundEngine.Item.Container {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Inventory;
	using SoulboundEngine.Loot;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Player;
	using System.Collections.Generic;

#nullable enable

	public abstract class LootableContainerTileEntity : TileEntity, IInventoryScreenHandlerFactory, ILootableInventory {
		protected RegistryKey<LootTable>? lootTable;
		protected long lootTableSeed;

		protected LootableContainerTileEntity(TileEntityType tileEntityType, BlockPos blockPos, BlockState blockState)
			: base(tileEntityType, blockPos, blockState) {
		}

		public virtual bool CanPlayerUse(PlayerEntity player) {
			return true;
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
			this.UnpackLootTable(player);
		}

		public virtual void OnClosed(PlayerEntity player) {
		}

		public override void WriteAdditional(JObject json) {
			base.WriteAdditional(json);
			if (!this.TrySaveLootTable(json)) {
				json["contents"] = this.Save();
			}
		}

		public override void ReadAdditional(JObject json) {
			base.ReadAdditional(json);
			if (!this.TryLoadLootTable(json)) {
				JToken? contentsJson = json["contents"];
				if (contentsJson == null) {
					Logger.LogError("No contents property found on LootableContainerTileEntity json: {}", json);
					return;
				}
				this.Load(contentsJson);
			}
		}

	}
}
