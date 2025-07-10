using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IItemSlot : IPointerDownHandler {
	public ItemDisplay ItemDisplay { get; }
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;
	public ItemStack ItemStack => ItemDisplay?.ItemStack;
	public GameObject GameObject { get; }
	
	public void OnClick(ItemDisplay grabbedItem, InventoryController inventory);
	
	// PLANNED REFACTOR: attach and detach slot methods will cause problems later on with serializations

	public virtual void AttachItemDisplay(ItemDisplay itemDisplay) {
		itemDisplay?.transform.SetParent(GameObject.transform, true);
		itemDisplay.DisableGrab();
	}

	public virtual void DetachItemDisplay() {
		this.ItemDisplay.EnableGrab();
		this.ItemDisplay?.transform.SetParent(GameManager.GetPlayerInstance().Inventory.transform, true);
	}

}

public static class ItemSlotUtility {

	// PLANNED REFACTOR: RequestClickAction() should take pointer event data for further functionality with item management in slots
	public static void RequestClickAction(this IItemSlot slot) {
		InventoryController inventory = GameManager.GetPlayerInstance().Inventory;
		InputHandler.RequestAction(new("ItemDrag", 10, () => {
			InvocationHelper.If(slot.ValidClickAction(inventory.GrabbedItem), () => slot.OnClick(inventory.GrabbedItem, inventory));
		}));
		InputHandler.BlockContextUntil("ItemUse", () => GameManager.GetPlayerInstance().InputHandler.LeftHold);
	}

	public static bool ValidClickAction(this IItemSlot slot, ItemDisplay grabbedItem) => grabbedItem != null || slot.HasItem;

	public static void TranserItems(this IItemSlot slot, ItemDisplay grabbedItem, InventoryController inventory) {
		if (!inventory.PopupOpen) {
			return;
		}
		PlayerController player = GameManager.GetPlayerInstance();
		void SetDropCapabilities(bool enabled) {
			if (!enabled) {
				player.ItemUsageHandler.Disable(ItemUseTrigger.RightClick, ItemUseTrigger.RightHold);
				player.InputHandler.inputActions.Player.RightClick.performed += inventory.DropGrabbedItem;
			} else {
				player.ItemUsageHandler.Enable(ItemUseTrigger.RightClick, ItemUseTrigger.RightHold);
				player.InputHandler.inputActions.Player.RightClick.performed -= inventory.DropGrabbedItem;
			}
		}

		if (grabbedItem == null && slot.HasItem) { 
			SetDropCapabilities(false);
			player.SetMainHandItem(slot.ItemStack);
			inventory.GrabbedItem = slot.ItemDisplay;
			slot.DetachItemDisplay();
			return;
		} 

		if (grabbedItem != null && slot.IsEmpty) {
			SetDropCapabilities(true);
			((IItemSlot)slot).AttachItemDisplay(inventory.GrabbedItem);
			player.SetMainHandItem(inventory.Hotbar.ActiveSlot.ItemStack);
			inventory.GrabbedItem = null;
			return;
		}
		
		if (slot.ItemStack.Item != inventory.GrabbedItem.ItemStack.Item || (inventory.GrabbedItem.ItemStack.Quantity == slot.ItemStack.Quantity)) {
			ItemDisplay previousGrabbed = inventory.GrabbedItem;
			inventory.GrabbedItem = slot.ItemDisplay;
			slot.DetachItemDisplay();
			slot.AttachItemDisplay(previousGrabbed);
			player.SetMainHandItem(inventory.GrabbedItem.ItemStack);
		} else {
			int space = slot.ItemStack.Item.MaxStackSize - slot.ItemStack.Quantity;
			int transfer = Math.Min(space, inventory.GrabbedItem.ItemStack.Quantity);
			slot.ItemStack.Quantity += transfer;
			inventory.GrabbedItem.ItemStack.Quantity -= transfer;
			if (inventory.GrabbedItem.ItemStack.Quantity <= 0) {
				SetDropCapabilities(true);
				inventory.DestroyItemDisplay(inventory.GrabbedItem);
				inventory.GrabbedItem = null;
			}
		}
	}
}
