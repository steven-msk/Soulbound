using SoulboundEngine.Client.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public class Inventory : IItemContainer, IEnumerable<ItemStack> {
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

		public virtual bool CanPlayerUse(PlayerEntity player) => true;

		protected virtual ItemSlot CreateSlot(int index) => new(this, index);

		public IItemSlot GetSlot(int index) => this.slots[index];

		public IEnumerable<int> GetAllSlots() {
			List<int> list = new();
			for (int i = 0; i < this.slots.Length; i++) list.Add(i);
			return list;
		}

		private void UpdateUniqueItems(ItemStack oldStack, ItemStack newStack) {
			if (!newStack.IsEmpty() && !this.uniqueItems.Contains(newStack.item)) {
				this.uniqueItems.Add(newStack.item);
				onItemAdded?.Invoke(newStack.item);
			}
			if (!oldStack.IsEmpty()) {
				bool stillExists = false;
				foreach (var slot in this.slots) {
					if (slot.GetStack().IsOf(oldStack.item)) {
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

		public int GetSize() => this.slots.Length;

		public IEnumerator<ItemStack> GetEnumerator() {
			return this.slots.Select(s => s.GetStack()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
