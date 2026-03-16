using System;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class ItemStack {
		public event Action<int, int>? onQuantityChanged;
		public readonly Item item;
		public int quantity { get; private set; }

		internal ItemStack(Item item, int quantity) {
			this.item = item;
			this.quantity = Mathf.Clamp(quantity, 0, item.fullStackSize);
		}

		public bool IsFull() => quantity >= item.fullStackSize;
		public bool IsEmpty() => quantity <= 0;

		public void SetQuantity(int amount) {
			int oldQuantity = quantity;
			quantity = Mathf.Clamp(amount, 0, item.fullStackSize);
			OnQuantityChanged(oldQuantity, quantity);
		}

		// Try to add items, return how many were actually added
		public int Increment(int amount = 1) {
			if (amount <= 0) return 0;

			int added = Mathf.Min(GetSpaceLeft(), amount);
			quantity += added;
			OnQuantityChanged(quantity - added, quantity);
			return added;
		}

		// Try to remove items, returns how many were actually removed
		public int Decrement(int amount = 1) {
			if (amount <= 0) return 0;

			int removed = Mathf.Min(quantity, amount);
			quantity -= removed;
			OnQuantityChanged(quantity + removed, quantity);
			return removed;
		}

		private void OnQuantityChanged(int old, int @new) {
			onQuantityChanged?.Invoke(old, @new);
		}

		// FEATUREIMPL: dropped item stacks converging to avoid lag

		public int GetSpaceLeft() => item.fullStackSize - quantity;

		public bool IsStackableWith(ItemStack? itemStack) {
			return itemStack?.item == item && GetSpaceLeft() > 0;
		}

		public void FillFrom(ItemStack itemStack) {
			if (!IsStackableWith(itemStack)) return;

			int added = itemStack.Decrement(GetSpaceLeft());
			this.Increment(added);
		}

		public ItemStack Clone(int newQuantity) {
			// TODO: rework item stack cloning when data becomes a thing
			return new ItemStack(item, newQuantity);
		}

		public ItemStack Clone() => Clone(quantity);
	}
}
