using SoulboundEngine.Client.Players;
using System;
using System.Collections.Generic;


#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class Inventory : IItemContainer {
		public const int HOTBAR_SIZE = 9;
		public const int COLUMNS = 9;
		public const int ROWS = 3;
		private readonly ItemSlot[] slots = new ItemSlot[ROWS * COLUMNS + HOTBAR_SIZE];
		private int mainSlot = 0;
		private readonly HashSet<Item> uniqueItems = new();
		public event Action<Item>? onItemAdded;
		public event Action<Item>? onItemRemoved;
		public event Action<int, int>? mainSlotChanged;

		public Inventory() {
			for (int i = 0; i < this.slots.Length; i++) {
				ItemSlot slot = new(this, i);
				this.slots[i] = slot;

				slot.stackChanged += this.UpdateUniqueItems;
			}
			onItemAdded += item => {
				foreach (var uniqueItem in this.uniqueItems) {
					if (uniqueItem is IContainerItemListener containerListener) {
						containerListener.OnItemAdded(item, this);
					}
				}
			};
			onItemRemoved += item => {
				foreach (var uniqueItem in this.uniqueItems) {
					if (uniqueItem is IContainerItemListener containerListener) {
						containerListener.OnItemRemoved(item, this);
					}
				}
			};
		}

		public IItemSlot GetSlot(int index) => this.slots[index];

		public IReadOnlyList<int> GetAllSlots() {
			List<int> list = new();
			for (int i = 0; i < this.slots.Length; i++) list.Add(i);
			return list;
		}

		public IReadOnlyList<int> GetPopupSlots() {
			List<int> list = new();
			for (int i = 0; i < ROWS * COLUMNS; i++) list.Add(HOTBAR_SIZE + i);
			return list;
		}

		public IReadOnlyList<int> GetHotbarSlots() {
			List<int> list = new();
			for (int i = 0; i < HOTBAR_SIZE; i++) list.Add(i);
			return list;
		}

		private void UpdateUniqueItems(ItemStack? oldStack, ItemStack? newStack) {
			if (newStack != null && !this.uniqueItems.Contains(newStack.item)) {
				this.uniqueItems.Add(newStack.item);
				onItemAdded?.Invoke(newStack.item);
			}
			if (oldStack != null) {
				bool stillExists = false;
				foreach (var slot in this.slots) {
					if (slot.GetStack()?.item == oldStack.item) {
						stillExists = true;
						break;
					}
				}
				if (!stillExists) {
					this.uniqueItems.Remove(oldStack.item);
					onItemRemoved?.Invoke(oldStack.item);
				}
			}
		}

		public void OnOpened(Player player) {
		}

		public void OnClosed(Player player) {
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

		public int GetSize() => this.slots.Length;
	}
}
