using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public class HotbarController : MonoBehaviour, IItemContainer {
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

		private (InventorySlotHandle hotbarSlot, int key) active;
		public InventorySlotHandle ActiveSlot => active.hotbarSlot;
		public int ActiveKey => active.key;

		private InventoryController inventory = null!;
		public static int length => 9;

		private InventorySlotHandle[] slots = null!;
		public InventorySlotHandle[] Slots => slots;
		IReadOnlyList<IItemSlot> IItemContainer.slots => slots;

		public InventorySlotHandle this[int index] => slots[index];
		private static Dictionary<int, InventorySlotHandle> slotsByIndex = new();

		public void Construct(InventoryController inventory) {
			this.inventory = inventory;
			this.SetupGrid();
		}

		public void SetupGrid() {
			slots = gameObject.GetComponentsInChildren<InventorySlotHandle>();
			for (int i = 0; i < length; i++) {
				slots[i].index = i;
				slotsByIndex[i] = slots[i];
			}
		}

		public void SetActiveSlot(int slotKey) {
			UnityEngine.Debug.Assert(slotKey >= 0 && slotKey < length, $"Unexpected hotbar slotKey {slotKey}");
			InventorySlotHandle hotbarSlot = this[slotKey];
			if (active.hotbarSlot == null) {
				active = (hotbarSlot, slotKey);
				ApplySelectionChanges(active.hotbarSlot, activeSlotColor, activeSlotNumberColor, activeItemStackColor, activeSlotOffset, activeSlotScale);
			}

			if (hotbarSlot != active.hotbarSlot) {
				if (!inventory.IsOpened) {
					ApplySelectionChanges(active.hotbarSlot, inactiveSlotColor, inactiveSlotNumberColor, inactiveItemStackColor, -activeSlotOffset, Vector3.one);
				} else {
					ApplySelectionChanges(active.hotbarSlot, activeSlotColor, activeSlotNumberColor, activeItemStackColor, -activeSlotOffset, Vector3.one);
				}
				active = (hotbarSlot, slotKey);
				ApplySelectionChanges(active.hotbarSlot, activeSlotColor, activeSlotNumberColor, activeItemStackColor, activeSlotOffset, activeSlotScale);
			}

			activeItemText.text = slots[slotKey].stack?.item.name ?? "";
			inventory.EquipHotbarItem(slots[slotKey]);
		}

		public void OnHotbarScroll(float scrollDelta) {
			int currentSlot = active.key;
			UnityEngine.Debug.Assert(currentSlot >= 0 && currentSlot < length);

			int nextSlot = currentSlot - (int)scrollDelta;
			SetActiveSlot(Mathf.Clamp(nextSlot, 0, length - 1));
		}

		private void ApplySelectionChanges(InventorySlotHandle slot, Color color, Color slotNumberColor, Color itemStackColor, Vector3 offset, Vector3 scale) {
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
			if (!slot.IsEmpty && slot.stack.item.IsStackable) {
				slot.itemDisplay.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = itemStackColor;
			}
		}

		public void OnInventoryPopup() {
			Color slotColor = inventory.IsOpened ? activeSlotColor : inactiveSlotColor;
			Color slotNumberColor = inventory.IsOpened ? activeSlotNumberColor : inactiveSlotNumberColor;
			Color itemStackColor = inventory.IsOpened ? activeItemStackColor : inactiveItemStackColor;
			foreach (var slot in slots.Where(slot => !slot.Equals(active.hotbarSlot))) {
				ApplySelectionChanges(slot, slotColor, slotNumberColor, itemStackColor, Vector3.zero, Vector3.one);
			}
			inventory.EquipHotbarItem(active.hotbarSlot);
		}

		public void OnItemTransfer(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			activeItemText.text = active.hotbarSlot.stack?.item.name ?? "";
			if (slot == (IItemSlot)active.hotbarSlot) {
				if (slot.IsEmpty && grabbedItem.value != null) {
					grabbedItem.value.transform.localScale = Vector3.one;
				} else if (slot.HasStack() && grabbedItem.value == null) {
					slot.itemDisplay.transform.localScale = Vector3.one;
				}
			}
		}

		public InventorySlotHandle GetSlotByIndex(int index) => slotsByIndex[index];

		public void OnPointerDown(IItemSlot slot, PointerEventData eventData) {
			throw new NotImplementedException();
		}

		public void OnPointerUp(IItemSlot slot, PointerEventData eventData) {
			throw new NotImplementedException();
		}

		public void OnPointerEnter(IItemSlot slot, PointerEventData data) {
			throw new NotImplementedException();
		}

		public void OnPointerExit(IItemSlot slot, PointerEventData data) {
			throw new NotImplementedException();
		}

        public void OnItemDisplayAdded(ItemDisplay itemDisplay, IItemSlot slot) {
            throw new NotImplementedException();
        }

		public IItemSlot GetSlot(int index) {
			return ((IItemContainerDomain)inventory).GetSlot(index);
		}

		public IReadOnlyList<int> GetAllSlots() {
			return ((IItemContainer)inventory).GetAllSlots();
		}

		public int GetSlotCount() {
			return ((IItemContainer)inventory).GetSlotCount();
		}
	}
}
