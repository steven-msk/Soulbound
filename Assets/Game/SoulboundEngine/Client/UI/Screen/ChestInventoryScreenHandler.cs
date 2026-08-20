using SoulboundEngine.Item;
using SoulboundEngine.Item.Container;
using SoulboundEngine.World.Player;
using SoulboundEngine.World.Block.Entity;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.UI.Screen {
	public class ChestInventoryScreenHandler : InventoryScreenHandler {
		private readonly IInventory chestInventory;
		private readonly PlayerInventory playerInventory;

		public ChestInventoryScreenHandler(InventoryScreenHandlerType<ChestInventoryScreenHandler> type, PlayerInventory playerInventory, IInventory chestInventory)
			: base(type) {
			this.chestInventory = chestInventory;
			this.playerInventory = playerInventory;
			this.AddPlayerSlots(playerInventory);
			foreach (var slot in chestInventory.GetAllSlots()) {
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
			List<IItemSlot> playerSlots = this.playerInventory.GetAllSlots().ToList();

			ItemStack slotStack = slot.GetStack();
			IInventory targetInventory = playerSlots.Contains(slot) ? this.chestInventory : this.playerInventory;
			InventoryUtils.TryAddStack(targetInventory, ref slotStack);
			slot.SetStack(slotStack);
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
