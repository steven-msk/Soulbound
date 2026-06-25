using SoulboundEngine.Client.Player;
using SoulboundEngine.Core.Assets;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public class DefaultInventoryScreen : InventoryScreen<DefaultInventoryScreenHandler> {
		public DefaultInventoryScreen(DefaultInventoryScreenHandler handler, PlayerInventory playerInventory) 
			: base(handler, playerInventory, AssetManager.Resolve<VisualTreeAsset>(new AssetKey("InventoryContextScreen"))) {
		}

		protected override void OnBind(VisualElement root) {
		}
	}
}
