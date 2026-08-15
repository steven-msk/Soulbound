using SoulboundEngine.Item.Container;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Registry;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI {
	public class ChestInventoryScreen : InventoryScreen<ChestInventoryScreenHandler> {
		private static readonly Identifier CHEST_SPACE_ELEMENT = Identifier.Of("soulbound:chest_inventory_screen/chest_space");
		private static readonly Identifier HOTBAR_ELEMENT = Identifier.Of("soulbound:hotbar/hotbar");
		private static readonly Identifier POPUP_ELEMENT = Identifier.Of("soulbound:player_inventory/popup");
		private static readonly Identifier PLAYER_INVENTORY_SPACE_ELEMENT = Identifier.Of("soulbound:chest_inventory_screen/player_inventory_space");

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

				this.BindSlot(slotElement, slot, chestInventory, true);
			}
		}

		private VisualElement GetChestRoot(VisualElement inventoryRoot) {
			return inventoryRoot.Get<VisualElement>(CHEST_SPACE_ELEMENT);
		} 

		protected override VisualElement GetPlayerHotbar(VisualElement inventoryRoot) {
			return inventoryRoot.Get<VisualElement>(HOTBAR_ELEMENT);
		}

		protected override VisualElement GetPlayerPopup(VisualElement inventoryRoot) {
			return inventoryRoot.Get<VisualElement>(POPUP_ELEMENT);
		}

		protected override VisualElement GetPlayerInventoryRoot(VisualElement screenRoot) {
			return screenRoot.Get<VisualElement>(PLAYER_INVENTORY_SPACE_ELEMENT);
		}
	}
}
