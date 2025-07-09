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

	public void OnPointerDown(PointerEventData eventData) {
		InputHandler.RequestAction(new("ItemDrag", 10, () => GameManager.GetPlayerInstance().Inventory.OnSlotClick(this)));
		InputHandler.BlockContextUntil("ItemUse", () => GameManager.GetPlayerInstance().InputHandler.LeftHold);
	}
}