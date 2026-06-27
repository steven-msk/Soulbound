using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.UI.Screen;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI {
	public class ChestInventoryScreen : InventoryScreen<ChestInventoryScreenHandler> {
		public ChestInventoryScreen(Context ctx, VisualTreeAsset asset) 
			: base(ctx, asset) {
		}

		protected override void OnBindInventory(VisualElement root) {
			base.OnBindInventory(root);

			IInventory chestInventory = this.handler.GetChestInventory();
			VisualElement chestRoot = this.GetChestRoot(root);

			foreach (var slotIndex in chestInventory.GetSlots()) {
				IItemSlot slot = chestInventory.GetSlot(slotIndex);
				VisualElement slotElement = chestRoot[slotIndex];

				this.BindSlot(slotElement, slot, chestInventory);
			}
		}

		private VisualElement GetChestRoot(VisualElement inventoryRoot) {
			return inventoryRoot.Q<VisualElement>("ChestSpace");
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
