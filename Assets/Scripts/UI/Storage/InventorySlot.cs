using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IItemSlot {

	// FEATUREIMPL (PARTIALLY IMPLEMENTED): equipment slots (NOT TESTED)

	public ItemDisplay ItemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;
	public ItemStack ItemStack => ItemDisplay?.ItemStack;

	public GameObject GameObject => gameObject;

	public void OnClick(ItemDisplay grabbedItem, InventoryController inventory) => this.TranserItems(grabbedItem, inventory);

	public void OnPointerDown(PointerEventData eventData) {
		InventoryController inventory = GameManager.GetPlayerInstance().Inventory;
		InputHandler.RequestAction(new("ItemDrag", 10, () => this.OnClick(inventory.GrabbedItem, inventory)));
		InputHandler.BlockContextUntil("ItemUse", () => GameManager.GetPlayerInstance().InputHandler.LeftHold);
	}
}