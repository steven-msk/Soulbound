namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.Inventory;

	public interface IInventoryScreenHandlerProvider<THandler> where THandler : InventoryScreenHandler {
		THandler GetScreenHandler();
	}
}
