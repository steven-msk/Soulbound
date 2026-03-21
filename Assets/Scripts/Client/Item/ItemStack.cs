using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class ItemStack {
		public event Action<int, int>? onQuantityChanged;
		public readonly Item item;
		private readonly Dictionary<Type, IItemStackData> data = new();
		public int quantity { get; private set; }

		internal ItemStack(Item item, int quantity) {
			this.item = item;
			this.quantity = Mathf.Clamp(quantity, 0, item.fullStackSize);
		}

		private ItemStack(Item item, int quantity, Dictionary<Type, IItemStackData> data)
			: this(item, quantity) {
			this.data = data;
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
			return itemStack?.item == item
				&& GetData().SequenceEqual(itemStack.GetData());
		}

		public void FillFrom(ItemStack itemStack) {
			if (!IsStackableWith(itemStack)) return;

			int added = itemStack.Decrement(GetSpaceLeft());
			this.Increment(added);
		}

		public T GetData<T>() where T : IItemStackData {
			return (T)data[typeof(T)];
		}

		public void SetData<T>(T data) where T : IItemStackData {
			this.data[typeof(T)] = data;
		}

		public IEnumerable<IItemStackData> GetData() => data.Values;
	
		public ItemStack Clone(int newQuantity) {
			return new ItemStack(item, newQuantity, data);
		}

		public ItemStack Clone() => Clone(quantity);
	}
}
