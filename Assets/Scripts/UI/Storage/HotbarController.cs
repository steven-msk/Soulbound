using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour {

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

	[Header("Internal")]
	[SerializeField] private InventorySlot activeSlot;
	public InventorySlot ActiveSlot => activeSlot;	

	public readonly Dictionary<int, InventorySlot> slots = new();

	private void Awake() {
		InventorySlot[] hotbarSlots = gameObject.GetComponentsInChildren<InventorySlot>();
		for (int i = 0; i < hotbarSlots.Length; i++) {
			hotbarSlots[i].SlotNumber = i + 1;
			slots[i + 1] = hotbarSlots[i];
		}
	}

	private void Start() {
		SetActiveSlot(1);           // temporary
	}

	public void SetActiveSlot(int slotKey) {
		Debug.Assert(slotKey >= 1 && slotKey <= slots.Count, $"Unexpected slotKey {slotKey}");
		InventorySlot slot = slots[slotKey];
		if (activeSlot == null) {
			activeSlot = slot;
			ApplySelectionChanges(activeSlot, activeSlotColor, activeSlotNumberColor, activeItemStackColor, activeSlotOffset, activeSlotScale);
		}

		if (slot != activeSlot) {
			if (!GameManager.GetPlayerInstance().Inventory.PopupOpen) {
				ApplySelectionChanges(activeSlot, inactiveSlotColor, inactiveSlotNumberColor, inactiveItemStackColor, -activeSlotOffset, Vector3.one);
			} else {
				ApplySelectionChanges(activeSlot, activeSlotColor, activeSlotNumberColor, activeItemStackColor, -activeSlotOffset, Vector3.one);
			}
			activeSlot = slot;
			ApplySelectionChanges(activeSlot, activeSlotColor, activeSlotNumberColor, activeItemStackColor, activeSlotOffset, activeSlotScale);
		}
		GameManager.GetPlayerInstance().Inventory.EquipHotbarItem(slots[slotKey]);
	}

	public void OnHotbarScroll(float scrollDelta) {
		int currentSlot = activeSlot.SlotNumber;
		Debug.Assert(currentSlot >= 1 && currentSlot <= slots.Count);

		int nextSlot = currentSlot - (int)scrollDelta;
		SetActiveSlot(Mathf.Clamp(nextSlot, 1, slots.Count));
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
		if (!IsEmpty(slot.GetComponent<InventorySlot>())) {
			slot.GetComponentInChildren<ItemDisplay>().gameObject.GetComponentInChildren<TextMeshProUGUI>().color = itemStackColor;
		}
	}

	public void OnInventoryPopup() {
		InventoryController inventory = GameManager.GetPlayerInstance().Inventory;
		Color slotColor = inventory.PopupOpen ? activeSlotColor : inactiveSlotColor;
		Color slotNumberColor = inventory.PopupOpen ? activeSlotNumberColor : inactiveSlotNumberColor;
		Color itemStackColor = inventory.PopupOpen ? activeItemStackColor : inactiveItemStackColor;
		foreach (var slotEntry in slots.Where(slotEntry => !slotEntry.Value.Equals(activeSlot))) {
			ApplySelectionChanges(slotEntry.Value, slotColor, slotNumberColor, itemStackColor, Vector3.zero, Vector3.one);
		}
	}

	public bool IsEmpty(InventorySlot slot) => slot.GetComponentInChildren<ItemDisplay>() == null;
}