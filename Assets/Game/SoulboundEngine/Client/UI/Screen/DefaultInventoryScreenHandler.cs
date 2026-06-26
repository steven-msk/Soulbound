using SoulboundEngine.Client.Player;

namespace SoulboundEngine.Client.UI.Screen {
	public class DefaultInventoryScreenHandler : InventoryScreenHandler {
		public DefaultInventoryScreenHandler(PlayerInventory playerInventory) 
			: base(InventoryScreenHandlerType.DEFAULT_INVENTORY) {
			this.AddPlayerSlots(playerInventory);
		}

		public override bool CanUse(PlayerEntity player) => true;
	}
}
