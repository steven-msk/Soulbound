using SoulboundEngine.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class Hotbar : IItemContainer {
		public const int SLOT_COUNT = 9;

		private readonly ItemSlot[] slots = new ItemSlot[SLOT_COUNT];
		private int mainSlotIndex= 0;
		public event Action<int, int> mainSlotChanged;

		public Hotbar() {
			for (int i = 0; i < SLOT_COUNT; i++) {
				slots[i] = new ItemSlot(this, i);
			}
		}

		public IReadOnlyList<int> GetAllSlots() {
			List<int> list = new();
			for (int i = 0; i < SLOT_COUNT; i++) list.Add(i);
			return list;
		}

		public IItemSlot GetSlot(int index) {
			if (index < 9) return slots[index];
			throw new ArgumentException("Hotbar slot index out of range: " + index);
		}

		public int GetSlotCount() => SLOT_COUNT;

		public int GetMainSlotIndex() => mainSlotIndex;
		public void SetMainSlotIndex(int index) {
			int previous = mainSlotIndex;
			mainSlotIndex = index;
			mainSlotChanged?.Invoke(previous, index);
		}
	}
}
