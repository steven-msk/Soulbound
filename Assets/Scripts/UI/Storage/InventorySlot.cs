using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IItemSlot {

	// FEATUREIMPL (PARTIALLY IMPLEMENTED): equipment slots (NOT TESTED)

	public IItemContainer2D container => gameObject.GetComponentInParent<InventoryController>(true);
	public ItemDisplay ItemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
	public int index { get; set; }
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;

	public ItemStack ItemStack => ItemDisplay?.ItemStack;

	public GameObject GameObject => gameObject;

	public void Deserialize(SerializedItemSlot serialized) {
		ItemDisplay.Create(serialized.itemStack, this);
	}

	[Obsolete]
	public void OnClick(ItemDisplay grabbedItem, InventoryController inventory) => this.TransferGrabbed(grabbedItem, inventory);

	//public void OnPointerDown(PointerEventData eventData) => this.RequestClickAction();
}