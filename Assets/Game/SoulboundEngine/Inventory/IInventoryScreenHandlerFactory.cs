namespace SoulboundEngine.Inventory {
	using SoulboundEngine.World.Player;

	public interface IInventoryScreenHandlerFactory {
		InventoryScreenHandler Create(PlayerInventory playerInventory, PlayerEntity player);
	}
}
