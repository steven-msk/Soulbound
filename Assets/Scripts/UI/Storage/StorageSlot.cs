using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class StorageSlot : MonoBehaviour, IPointerDownHandler {

	public ItemDisplay ItemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;
	public ItemStack ItemStack => ItemDisplay?.ItemStack;

	[Obsolete] public bool EditMode {  get; set; }

	public int SlotNumber {  get; set; }

	public void OnPointerDown(PointerEventData eventData) {
		GameManager.GetPlayerInstance().Inventory.OnSlotClick(this);
	}
}