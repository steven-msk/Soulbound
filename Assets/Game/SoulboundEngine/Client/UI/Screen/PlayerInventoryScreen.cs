using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public class PlayerInventoryScreen : InventoryScreen<PlayerInventoryScreenHandler> {
		public PlayerInventoryScreen(Context ctx, VisualTreeAsset asset) 
			: base(ctx, asset) {
		}

		protected override VisualElement GetPlayerHotbar(VisualElement inventoryRoot) {
			return inventoryRoot.Q<VisualElement>("Hotbar");
		}

		protected override VisualElement GetPlayerPopup(VisualElement inventoryRoot) {
			return inventoryRoot.Q<VisualElement>("Popup");
		}

		protected override VisualElement GetPlayerInventoryRoot(VisualElement screenRoot) {
			return screenRoot.Q<VisualElement>("PlayerInventorySpace");
		}
	}
}
