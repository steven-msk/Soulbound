using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HotbarController : MonoBehaviour, IHotbarContainer, IDependencyInitializable<HotbarController, InventoryController> {
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

	[Header("")]
	[SerializeField] private TextMeshProUGUI activeItemText;

	private (InventorySlot hotbarSlot, int key) active;
	public InventorySlot ActiveSlot => active.hotbarSlot;

	private InventoryController inventory;

	public int Columns => 9;

	private InventorySlot[] slots;
	public InventorySlot[] Slots => slots;
	public InventorySlot this[int index] => slots[index];

#nullable enable

	public HotbarController OnGameInit(InventoryController dependency) {
		inventory = dependency;
		this.SetupGrid();
		return this;
	}

	public void SetupGrid() {
		InventorySlot[] hotbarSlots = gameObject.GetComponentsInChildren<InventorySlot>();
		slots = new InventorySlot[Columns];
		for (int i = 0; i < Columns; i++) {
			slots[i] = hotbarSlots[i];
		}
	}

	public void SetActiveSlot(int slotKey) {
		UnityEngine.Debug.Assert(slotKey >= 0 && slotKey < Columns, $"Unexpected hotbar slotKey {slotKey}");
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

		activeItemText.text = slots[slotKey].ItemStack?.Item.name;
		GameManager.instance.Player.Inventory.EquipHotbarItem(slots[slotKey]);
	}

	public void OnHotbarScroll(float scrollDelta) {
		int currentSlot = active.key;
		UnityEngine.Debug.Assert(currentSlot >= 0 && currentSlot < Columns);

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