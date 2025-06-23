using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IItemSlot {

	// FEATUREIMPL (PARTIALLY IMPLEMENTED): equipment slots (NOT TESTED)

	public ItemDisplay ItemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;
	public ItemStack ItemStack => ItemDisplay?.ItemStack;

	// TODO: remove editMode field of InventorySlot
	[Obsolete] public bool EditMode {  get; set; }

	public int SlotNumber { get; set; }

	public GameObject GameObject => gameObject;

	public void OnPointerDown(PointerEventData eventData) => GameManager.GetPlayerInstance().Inventory.OnSlotClick(this);
}