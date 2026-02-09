using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.ItemSystem {
	public sealed class PlayerInventory : IItemContainer, IItemContainerDomain {
		public const int ROWS = 9;
		public const int COLUMNS = 4;
		public const int POPUP_COLUMNS = 3;
		public const int HOTBAR_ROWS = ROWS;

		private readonly InventorySlot[] popupSlots = new InventorySlot[ROWS * POPUP_COLUMNS];
		private readonly InventorySlot[] hotbarSlots = new InventorySlot[HOTBAR_ROWS];
		//private readonly InventorySlot[] armorSlots = new InventorySlot[4];	// TBD
		private bool popupOpen = false;
		public event Action togglePopup;

		public PlayerInventory() {
			int i = 0;
			for (; i < ROWS * POPUP_COLUMNS; i++) popupSlots[i] = new InventorySlot(this, i);
			for (; i < ROWS * POPUP_COLUMNS + HOTBAR_ROWS; i++) {
				hotbarSlots[i - ROWS * POPUP_COLUMNS] = new InventorySlot(this, i);
			}
			//for (; i < 39; i++) armorSlots[i - 36] = new ArmorSlot(this, i);
		}

		public IItemSlot GetSlot(int index) {
			if (index < ROWS * POPUP_COLUMNS) return popupSlots[index];
			if (index < ROWS * POPUP_COLUMNS + HOTBAR_ROWS) return hotbarSlots[index - ROWS * POPUP_COLUMNS];
			//else if (index < 39) return armorSlots[index];
			throw new ArgumentException("Slot index out of range: " + index);
		}

		public InventorySlot[][] GetPopupGrid() {
			InventorySlot[][] grid = new InventorySlot[POPUP_COLUMNS][];
			int index = 0;

			for (int i = 0; i < POPUP_COLUMNS; i++) {
				grid[i] = new InventorySlot[ROWS];

				for (int j = 0; j < ROWS; j++) {
					grid[i][j] = popupSlots[index++];
				}
			}
			return grid;
		}

		public IReadOnlyList<IItemSlot> GetAllSlots() {
			return new List<IItemSlot>()
				.Concat(popupSlots)
				.Concat(hotbarSlots)
				.ToList();
		}

		public IReadOnlyList<int> GetAllSlots_indexed() {
			List<int> list = new();
			int i = 0;
			for (; i < ROWS * POPUP_COLUMNS; i++) list.Add(i);
			for (; i < ROWS * POPUP_COLUMNS + HOTBAR_ROWS; i++) list.Add(i);
			return list;
		}

		public void TogglePopup() {
			popupOpen = !popupOpen;
			togglePopup();
		}

		public InventorySlot[] GetPopup() => popupSlots;
		public InventorySlot[] GetHotbar() => hotbarSlots;

		public bool IsPopupOpen() => popupOpen;

		void IItemContainer.OnItemDisplayAdded(ItemDisplay itemDisplay, IItemSlot slot) {
			throw new NotImplementedException();
		}

		[Obsolete] public IReadOnlyList<IItemSlot> slots { get => throw new NotImplementedException(); }
	}
}
