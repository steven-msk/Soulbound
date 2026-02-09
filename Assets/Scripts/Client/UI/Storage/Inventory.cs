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
	public sealed class Inventory : IItemContainer, IItemContainerDomain {
		public const int COLUMNS = 9;
		public const int ROWS = 3;
		private readonly InventorySlot[] slots = new InventorySlot[ROWS * COLUMNS];
		private bool isOpen = false;
		public event Action toggle;

		public Inventory() {
			for (int i = 0; i < ROWS * COLUMNS; i++) {
				slots[i] = new InventorySlot(this, i);
			}

			// prototypical
			slots[0].SetStack(new(Items.woodBlock, 10));
			slots[1].SetStack(new(Items.leavesBlock, 100));
			slots[2].SetStack(new(Items.leavesBlock, 256));
			slots[3].SetStack(new(Items.leavesBlock, 256));
		}

		public IItemSlot GetSlot(int index) {
			if (index < ROWS * COLUMNS) return slots[index];
			throw new ArgumentException("Slot index out of range: " + index);
		}

		public IReadOnlyList<IItemSlot> GetAllSlots() {
			return new List<IItemSlot>().Concat(slots).ToList();
		}

		public IReadOnlyList<int> GetAllSlots_indexed() {
			List<int> list = new();
			int i = 0;
			for (; i < ROWS * COLUMNS; i++) list.Add(i);
			return list;
		}

		public void Toggle() {
			isOpen = !isOpen;
			toggle();
		}

		public bool IsOpen() => isOpen;

		void IItemContainer.OnItemDisplayAdded(ItemDisplay itemDisplay, IItemSlot slot) {
			throw new NotImplementedException();
		}
		IReadOnlyList<IItemSlot> IItemContainer.slots => throw new NotImplementedException();
	}
}
