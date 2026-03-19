using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem.Container {
	public sealed class Inventory : IItemContainer {
		public const int COLUMNS = 9;
		public const int ROWS = 3;
		private readonly ItemSlot[] slots = new ItemSlot[ROWS * COLUMNS];
		private bool isOpen = true;
		private readonly HashSet<Item> uniqueItems = new();
		public event Action<Item>? onItemAdded;
		public event Action<Item>? onItemRemoved;
		public event Action toggle = null!;

		public Inventory() {
			for (int i = 0; i < ROWS * COLUMNS; i++) {
				ItemSlot slot = new(this, i);
				slots[i] = slot;

				slot.stackChanged += UpdateUniqueItems;
			}
			onItemAdded += item => {
				foreach (var uniqueItem in uniqueItems) {
					if (uniqueItem is IContainerItemListener containerListener) {
						containerListener.OnItemAdded(item, this);
					}
				}
			};
			onItemRemoved += item => {
				foreach (var uniqueItem in uniqueItems) {
					if (uniqueItem is IContainerItemListener containerListener) {
						containerListener.OnItemRemoved(item, this);
					}
				}
			};
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

		private void UpdateUniqueItems(ItemStack? oldStack, ItemStack? newStack) {
			if (newStack != null && !uniqueItems.Contains(newStack.item)) {
				uniqueItems.Add(newStack.item);
				onItemAdded?.Invoke(newStack.item);
			}
			if (oldStack != null) {
				bool stillExists = false;
				foreach (var slot in slots) {
					if (slot.GetStack()?.item == oldStack.item) {
						stillExists = true;
						break;
					}
				}
				if (!stillExists) {
					uniqueItems.Remove(oldStack.item);
					onItemRemoved?.Invoke(oldStack.item);
				}
			}
		}

		public bool IsOpen() => isOpen;

		public int GetSlotCount() => ROWS * COLUMNS;
	}
}
