using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Player;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.UI.Screen {
	public class ChestInventoryScreenHandler : InventoryScreenHandler {
		private readonly Inventory chestInventory;
		private readonly PlayerInventory playerInventory;

		public ChestInventoryScreenHandler(InventoryScreenHandlerType type, PlayerInventory playerInventory, Inventory chestInventory)
			: base(type) {
			this.chestInventory = chestInventory;
			this.playerInventory = playerInventory;
			this.AddPlayerSlots(playerInventory);
			foreach (var slot in chestInventory.GetAllSlots().Select(i => chestInventory.GetSlot(i))) {
				this.AddSlot(slot);
			}
		}

		public override bool CanUse(PlayerEntity player) {
			return this.chestInventory.CanPlayerUse(player);
		}

		protected override void QuickMove(PlayerEntity player, IItemSlot slot) {
			List<IItemSlot> playerSlots = this.playerInventory.GetAllSlots().Select(i => this.playerInventory.GetSlot(i)).ToList();
			List<IItemSlot> chestSlots = this.chestInventory.GetAllSlots().Select(i => this.chestInventory.GetSlot(i)).ToList();

			ItemStack slotStack = slot.GetStack();
			InventoryUtils.TryAddStack(playerSlots.Contains(slot) ? this.chestInventory : this.playerInventory, ref slotStack);
			slot.SetStack(slotStack);
		}
	}
}
