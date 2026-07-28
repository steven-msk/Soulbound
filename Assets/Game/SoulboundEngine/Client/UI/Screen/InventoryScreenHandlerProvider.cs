namespace SoulboundEngine.Client.UI.Screen {
	public interface InventoryScreenHandlerProvider<THandler> where THandler : InventoryScreenHandler {
		THandler GetScreenHandler();
	}
}
