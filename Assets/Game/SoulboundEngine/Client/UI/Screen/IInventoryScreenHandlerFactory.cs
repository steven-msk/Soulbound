using SoulboundEngine.Client.Player;

namespace SoulboundEngine.Client.UI.Screen {
	public interface IInventoryScreenHandlerFactory {
		InventoryScreenHandler Create(PlayerInventory playerInventory, PlayerEntity player);
	}
}
