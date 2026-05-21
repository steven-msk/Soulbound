using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	using Player = Player.Player;

	public class Inventory : IItemContainer, IEnumerable<ItemStack?> {
		protected readonly ItemSlot[] slots;
		private readonly HashSet<Item> uniqueItems = new();
		public event Action<Item>? onItemAdded;
		public event Action<Item>? onItemRemoved;

		public Inventory(int size) {
			this.slots = new ItemSlot[size];

			for (int i = 0; i < size; i++) {
				ItemSlot slot = this.CreateSlot(i);
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

		protected virtual ItemSlot CreateSlot(int index) => new(this, index);

		public IItemSlot GetSlot(int index) => this.slots[index];

		public IEnumerable<int> GetAllSlots() {
			List<int> list = new();
			for (int i = 0; i < this.slots.Length; i++) list.Add(i);
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

		public virtual void OnOpened(Player player) {
		}

		public virtual void OnClosed(Player player) {
		}

		public int GetSize() => this.slots.Length;

		public IEnumerator<ItemStack?> GetEnumerator() {
			return this.slots.Select(s => s.GetStack()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
