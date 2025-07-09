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

public static class ItemTransferUtility {
	public static void TranserItems(this IItemSlot slot, ItemDisplay grabbedItem, InventoryController inventory) {
		PlayerController player = GameManager.GetPlayerInstance();
		if (!inventory.PopupOpen) {
			return;
		}
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
