using SoulboundEngine.Client.Player;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public abstract class InventoryScreen<THandler> : UxmlScreen, InventoryScreenHandlerProvider<THandler> where THandler : InventoryScreenHandler {
		protected readonly THandler handler;

		protected InventoryScreen(THandler handler, PlayerInventory playerInventory, VisualTreeAsset asset)
			: base(asset) {
			this.handler = handler;
		}

		protected abstract override void OnBind(VisualElement root);

		public THandler GetScreenHandler() => this.handler;
	}
}
