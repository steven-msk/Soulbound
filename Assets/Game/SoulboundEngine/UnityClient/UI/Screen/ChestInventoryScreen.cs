namespace SoulboundEngine.UnityClient.UI {
	using SoulboundEngine.Inventory;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.UnityClient.UI.Screen;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using UnityEngine.UIElements;

	public class ChestInventoryScreen : InventoryScreen<ChestInventoryScreenHandler> {
		private static readonly UXMLBinding<VisualElement> CHEST_SPACE_ELEMENT = new("soulbound:chest_inventory_screen/chest_space");
		private static readonly UXMLBinding<VisualElement> HOTBAR_ELEMENT = new("soulbound:hotbar/hotbar");
		private static readonly UXMLBinding<VisualElement> POPUP_ELEMENT = new("soulbound:player_inventory/popup");
		private static readonly UXMLBinding<VisualElement> PLAYER_INVENTORY_SPACE_ELEMENT = new("soulbound:chest_inventory_screen/player_inventory_space");

		public ChestInventoryScreen(Context ctx, VisualTreeAsset asset) 
			: base(ctx, asset) {
		}

		protected override void OnBindInventory(VisualElement root) {
			base.OnBindInventory(root);

			IInventory chestInventory = this.handler.GetChestInventory();
			VisualElement chestRoot = this.GetChestRoot(root);

			foreach (int slotIndex in chestInventory.GetSlots()) {
				IItemSlot slot = chestInventory.GetSlot(slotIndex);
				VisualElement slotElement = chestRoot[slotIndex];

				this.BindSlot(slotElement, slot, chestInventory, true);
			}
		}

		private VisualElement GetChestRoot(VisualElement inventoryRoot) {
			return CHEST_SPACE_ELEMENT.Get(inventoryRoot);
		} 

		protected override VisualElement GetPlayerHotbar(VisualElement inventoryRoot) {
			return HOTBAR_ELEMENT.Get(inventoryRoot);
		}

		protected override VisualElement GetPlayerPopup(VisualElement inventoryRoot) {
			return POPUP_ELEMENT.Get(inventoryRoot);
		}

		protected override VisualElement GetPlayerInventoryRoot(VisualElement screenRoot) {
			return PLAYER_INVENTORY_SPACE_ELEMENT.Get(screenRoot);
		}
	}
}
