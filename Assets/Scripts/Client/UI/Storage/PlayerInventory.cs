using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.ItemSystem {
	public sealed class PlayerInventory : IItemContainer {
		private readonly InventorySlot[] popupSlots = new InventorySlot[27];
		private readonly InventorySlot[] hotbarSlots = new InventorySlot[9];
		//private readonly InventorySlot[] armorSlots = new InventorySlot[4];	// TBD

		public PlayerInventory() {
			int i = 0;
			for (; i < 27; i++) popupSlots[i] = new InventorySlot(this, i);
			for (; i < 36; i++) hotbarSlots[i - 27] = new InventorySlot(this, i);
			//for (; i < 39; i++) {
			//	armorSlots[i] = new ArmorSlot(this, i);
			//}
		}

		public IReadOnlyList<IItemSlot> GetAllSlots() {
			List<IItemSlot> slots = new();
			slots.AddRange(popupSlots);
			slots.AddRange(hotbarSlots);
			return slots;
		}

		public IItemSlot GetSlotByIndex(int index) {
			if (index < 27) return popupSlots[index];
			if (index < 36) return hotbarSlots[index];
			//else if (index < 39) return armorSlots[index];
			throw new ArgumentException("Slot index out of range: " + index);
		}

		public InventorySlot[][] GetPopupGrid() {
			InventorySlot[][] grid = new InventorySlot[3][];
			int index = 0;

			for (int i = 0; i < 3; i++) {
				grid[i] = new InventorySlot[9];

				for (int j = 0; j < 9; j++) {
					grid[i][j] = popupSlots[index++];
				}
			}
			return grid;
		}

		public InventorySlot[] GetPopup() => popupSlots;
		public InventorySlot[] GetHotbar() => hotbarSlots;

		[Obsolete] public IReadOnlyList<IItemSlot> slots { get => throw new NotImplementedException(); }
		[Obsolete] public Transform transform => throw new NotImplementedException();
		[Obsolete]
		public void OnItemDisplayAdded(ItemDisplay itemDisplay, IItemSlot slot) {
			throw new NotImplementedException();
		}
		[Obsolete]
		public void OnPointerDown(IItemSlot slot, PointerEventData eventData) {
			throw new NotImplementedException();
		}
		[Obsolete]
		public void OnPointerEnter(IItemSlot slot, PointerEventData data) {
			throw new NotImplementedException();
		}
		[Obsolete]
		public void OnPointerExit(IItemSlot slot, PointerEventData data) {
			throw new NotImplementedException();
		}
		[Obsolete]
		public void OnPointerUp(IItemSlot slot, PointerEventData eventData) {
			throw new NotImplementedException();
		}
	}
}
