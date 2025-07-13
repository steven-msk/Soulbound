using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HotbarController : MonoBehaviour, IHotbarContainer {
	[Header("Active Slots")]
	public Color activeSlotColor;

	public Color activeSlotNumberColor;
	public Color activeItemStackColor;
	public Vector3 activeSlotOffset;
	public Vector3 activeSlotScale;

	[Header("Inactive Slots")]
	public Color inactiveSlotColor;

	public Color inactiveSlotNumberColor;
	public Color inactiveItemStackColor;

	private (InventorySlot hotbarSlot, int key) active;
	public InventorySlot ActiveSlot => active.hotbarSlot;

	private InventoryController inventory;

	public int Columns => 9;

	// FIXME: main hand item usage unavailable upon first trigger when entering play mode.
	// To reproduce this:
	// 1. Enter play mode
	// 2. Without changing the current hotbar slot, invoke the usage trigger of the item in the
	// hotbar slot
	// 3. If the item doesnt get used, switch to a different slot and go back to the original one;
	//    if now it gets used thats the bug

	private InventorySlot[] slots;
	public InventorySlot[] Slots => slots;
	public InventorySlot this[int index] => slots[index];

	private void Awake() {
		inventory = GameManager.instance.Player.Inventory;
		SetupGrid(() => inventory.SetupGrid(null));
		SetActiveSlot(0);
	}

#nullable enable

	public void SetupGrid(Action? callback) {
		InventorySlot[] hotbarSlots = gameObject.GetComponentsInChildren<InventorySlot>();
		slots = new InventorySlot[Columns];
		for (int i = 0; i < Columns; i++) {
			slots[i] = hotbarSlots[i];
		}
		callback?.Invoke();
	}

#nullable disable

	public void SetActiveSlot(int slotKey) {
		Debug.Assert(slotKey >= 0 && slotKey < Columns, $"Unexpected hotbar slotKey {slotKey}");
		InventorySlot hotbarSlot = this[slotKey];
		if (active.hotbarSlot == null) {
			active = (hotbarSlot, slotKey);
			ApplySelectionChanges(active.hotbarSlot, activeSlotColor, activeSlotNumberColor, activeItemStackColor, activeSlotOffset, activeSlotScale);
		}

		if (hotbarSlot != active.hotbarSlot) {
			if (!GameManager.instance.Player.Inventory.PopupOpen) {
				ApplySelectionChanges(active.hotbarSlot, inactiveSlotColor, inactiveSlotNumberColor, inactiveItemStackColor, -activeSlotOffset, Vector3.one);
			} else {
				ApplySelectionChanges(active.hotbarSlot, activeSlotColor, activeSlotNumberColor, activeItemStackColor, -activeSlotOffset, Vector3.one);
			}
			active = (hotbarSlot, slotKey);
			ApplySelectionChanges(active.hotbarSlot, activeSlotColor, activeSlotNumberColor, activeItemStackColor, activeSlotOffset, activeSlotScale);
		}

		// weird how this gets called in Awake() and sets the slot active, but doesnt actually equip the main hand item
		GameManager.instance.Player.Inventory.EquipHotbarItem(slots[slotKey]);
	}

	public void OnHotbarScroll(float scrollDelta) {
		int currentSlot = active.key;
		Debug.Assert(currentSlot >= 0 && currentSlot < Columns);

		int nextSlot = currentSlot - (int)scrollDelta;
		SetActiveSlot(Mathf.Clamp(nextSlot, 0, Columns - 1));
	}

	private void ApplySelectionChanges(InventorySlot slot, Color color, Color slotNumberColor, Color itemStackColor, Vector3 offset, Vector3 scale) {
		slot.transform.localScale = scale;
		slot.transform.localPosition += offset;
		Vector3 localPos = slot.transform.localPosition;
		Vector3 pixelSnappedPos = new(Mathf.Floor(localPos.x), Mathf.Floor(localPos.y), localPos.z);
		slot.transform.localPosition = pixelSnappedPos;
		slot.GetComponent<UnityEngine.UI.Image>().color = color;
		foreach (var hotbarSlot in slot.GetComponentsInChildren<Transform>()) {
			if (hotbarSlot.name != "Slot Number") {
				continue;
			}
			hotbarSlot.GetComponent<TextMeshProUGUI>().color = slotNumberColor;
			break;
		}
		if (!IsEmpty(slot.GetComponent<InventorySlot>()) && slot.ItemStack.Item.IsStackable) {
			slot.GetComponentInChildren<ItemDisplay>().gameObject.GetComponentInChildren<TextMeshProUGUI>().color = itemStackColor;
		}
	}

	public void OnInventoryPopup() {
		InventoryController inventory = GameManager.instance.Player.Inventory;
		Color slotColor = inventory.PopupOpen ? activeSlotColor : inactiveSlotColor;
		Color slotNumberColor = inventory.PopupOpen ? activeSlotNumberColor : inactiveSlotNumberColor;
		Color itemStackColor = inventory.PopupOpen ? activeItemStackColor : inactiveItemStackColor;
		foreach (var slot in slots.Where(slot => !slot.Equals(active.hotbarSlot))) {
			ApplySelectionChanges(slot, slotColor, slotNumberColor, itemStackColor, Vector3.zero, Vector3.one);
		}
		inventory.EquipHotbarItem(active.hotbarSlot);
	}

	public bool IsEmpty(InventorySlot slot) => slot.GetComponentInChildren<ItemDisplay>() == null;
}