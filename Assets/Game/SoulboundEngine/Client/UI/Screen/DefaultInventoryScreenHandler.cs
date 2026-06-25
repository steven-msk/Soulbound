using SoulboundEngine.Client.Player;

namespace SoulboundEngine.Client.UI.Screen {
	public class DefaultInventoryScreenHandler : InventoryScreenHandler {
		public DefaultInventoryScreenHandler() 
			: base(InventoryScreenHandlerType.DEFAULT_INVENTORY) {
		}

		public override bool CanUse(PlayerEntity player) => true;
	}
}
