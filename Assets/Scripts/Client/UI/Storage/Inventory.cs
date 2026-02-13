using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.ItemSystem {
	public sealed class Inventory : IItemContainer {
		public const int COLUMNS = 9;
		public const int ROWS = 3;
		private readonly ItemSlot[] slots = new ItemSlot[ROWS * COLUMNS];
		private bool isOpen = false;
		public event Action toggle;

		public Inventory() {
			for (int i = 0; i < ROWS * COLUMNS; i++) {
				slots[i] = new ItemSlot(this, i);
			}
		}

		public IItemSlot GetSlot(int index) {
			if (index < ROWS * COLUMNS) return slots[index];
			throw new ArgumentException("Slot index out of range: " + index);
		}

		public IReadOnlyList<int> GetAllSlots() {
			List<int> list = new();
			for (int i = 0; i < ROWS * COLUMNS; i++) list.Add(i);
			return list;
		}

		public void Toggle() {
			isOpen = !isOpen;
			toggle();
		}

		public bool IsOpen() => isOpen;

		public int GetSlotCount() => ROWS * COLUMNS;
	}
}
