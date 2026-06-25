using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Item.Container;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.Player {
	public sealed class PlayerInventory : Inventory {
		public const int HOTBAR_SIZE = 9;
		public const int POPUP_COLUMNS = 9;
		public const int POPUP_ROWS = 3;
		private int mainSlot = 0;
		public event Action<int, int>? mainSlotChanged;

		public PlayerInventory() 
			: base(POPUP_COLUMNS * POPUP_ROWS + HOTBAR_SIZE) {
		}

		public IEnumerable<int> GetPopup() {
			List<int> list = new();
			for (int i = 0; i < POPUP_ROWS * POPUP_COLUMNS; i++) list.Add(HOTBAR_SIZE + i);
			return list;
		}

		public IEnumerable<int> GetHotbar() {
			List<int> list = new();
			for (int i = 0; i < HOTBAR_SIZE; i++) list.Add(i);
			return list;
		}

		public int GetMainSlot() => this.mainSlot;

		public void SetMainSlot(int slot) {
			int previous = this.mainSlot;
			this.mainSlot = slot;
			mainSlotChanged?.Invoke(previous, slot);
		}

		public ItemStack? GetMainStack() {
			return this.slots[this.mainSlot].GetStack();
		}
	}
}
