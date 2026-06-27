using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Player;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.UI.Screen {
	public class DefaultInventoryScreenHandler : InventoryScreenHandler {
		private readonly PlayerInventory playerInventory;

		public DefaultInventoryScreenHandler(PlayerInventory playerInventory) 
			: base(InventoryScreenHandlerType.DEFAULT_INVENTORY) {
			this.AddPlayerSlots(playerInventory);
			this.playerInventory = playerInventory;
		}

		public override bool CanUse(PlayerEntity player) => true;

		protected override void QuickMove(PlayerEntity player, IItemSlot slot) {
			List<IItemSlot> hotbarSlots = this.playerInventory.GetHotbar().Select(i => this.playerInventory.GetSlot(i)).ToList();
			List<IItemSlot> popupSlots = this.playerInventory.GetPopup().Select(i => this.playerInventory.GetSlot(i)).ToList();

			ItemStack slotStack = slot.GetStack();
			ItemContainerUtils.TryAddStack(hotbarSlots.Contains(slot) ? popupSlots : hotbarSlots, ref slotStack);
			slot.SetStack(slotStack);
		}
	}
}
