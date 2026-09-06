namespace SoulboundEngine.Inventory {
	using SoulboundEngine.Item;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Player;

	public class ChestInventoryScreenHandler : InventoryScreenHandler {
		private readonly IInventory chestInventory;
		private readonly PlayerInventory playerInventory;

		public ChestInventoryScreenHandler(InventoryScreenHandlerType<ChestInventoryScreenHandler> type, PlayerInventory playerInventory, IInventory chestInventory)
			: base(type) {
			this.chestInventory = chestInventory;
			this.playerInventory = playerInventory;
			this.AddPlayerSlots(playerInventory);
			foreach (IItemSlot slot in chestInventory.GetAllSlots()) {
				this.AddSlot(slot);
			}
		}

		public ChestInventoryScreenHandler(InventoryScreenHandlerType<ChestInventoryScreenHandler> type, PlayerInventory playerInventory)
			: this(type, playerInventory, CreateInventory()) {
		}

		public override bool CanUse(PlayerEntity player) {
			return this.chestInventory.CanPlayerUse(player);
		}

		protected override void QuickMove(PlayerEntity player, IItemSlot slot) {
			PlayerInventoryScreenHandler.QuickMove(this, this.playerInventory, slot);
		}

		public override bool CanInsertIntoSlot(ItemStack itemStack, IItemSlot slot) {
			return PlayerInventoryScreenHandler.CanInsertIntoSlot(base.CanInsertIntoSlot, itemStack, slot);
		}

		public IInventory GetChestInventory() => this.chestInventory;

		private static IInventory CreateInventory() {
			return new SimpleInventory(ChestTileEntity.SIZE);
		}

		public override void OnClosed(PlayerEntity player) {
			this.chestInventory.OnClosed(player);
		}
	}
}
