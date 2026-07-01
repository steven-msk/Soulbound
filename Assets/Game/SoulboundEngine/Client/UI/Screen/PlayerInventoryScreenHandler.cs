using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Player;
using System.Linq;

namespace SoulboundEngine.Client.UI.Screen {
	public class PlayerInventoryScreenHandler : InventoryScreenHandler {
		private readonly PlayerInventory playerInventory;

		public PlayerInventoryScreenHandler(InventoryScreenHandlerType<PlayerInventoryScreenHandler> type, PlayerInventory playerInventory) 
			: base(type) {
			this.AddPlayerSlots(playerInventory);
			this.playerInventory = playerInventory;
		}

		public override bool CanUse(PlayerEntity player) => true;

		protected override void QuickMove(PlayerEntity player, IItemSlot slot) {
			IItemSlot[] hotbarSlots = this.playerInventory.GetHotbar().Select(this.playerInventory.GetSlot).ToArray();
			IItemSlot[] popupSlots = this.playerInventory.GetPopup().Select(this.playerInventory.GetSlot).ToArray();

			ItemStack slotStack = slot.GetStack();
			this.InsertItem(ref slotStack, hotbarSlots.Contains(slot) ? popupSlots : hotbarSlots, false);
			slot.SetStack(slotStack);
		}

		public override void OnContentChanged(IInventory inventory) {
			Logger.LogInfo("changed");
		}
	}
}
