using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

public class InventorySlot : MonoBehaviour, IItemSlot {
	public IItemContainer2D container => gameObject.GetComponentInParent<InventoryController>(true);
	public ItemDisplay? ItemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
	public int index { get; set; }
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;

	public ItemStack? ItemStack => ItemDisplay?.ItemStack;
	public bool showTooltip { get; set; } = true;

	public GameObject GameObject => gameObject;

	public void Deserialize(SerializedItemSlot serialized) {
		ItemDisplay.Create(serialized.itemStack, this);
	}

	public void OnInventoryPopup(bool opened) {
		if (!opened && this.HasItem) {
			ItemDisplay!.DestroyTooltip();
		}
	}
}