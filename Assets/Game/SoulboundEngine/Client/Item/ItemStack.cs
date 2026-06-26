using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Item {
	public struct ItemStack {
		public static readonly ItemStack EMPTY = Empty();
		public readonly Item item;
		public int count { get; private set; }

		internal ItemStack(Item item, int count) {
			this.item = item;
			this.count = count;
			this.CapCount(item.fullStackSize);
		}

		private static ItemStack Empty() {
			return new ItemStack() { count = 0 };
		}

		public readonly bool IsFull() => this.count >= this.item.fullStackSize;
		public readonly bool IsEmpty() => this.count <= 0 || this.IsOf(null);

		public readonly bool IsFullSize(int count) => count >= this.item.fullStackSize;

		/// <summary>
		/// Try to add items. Returns how may were actually added.
		/// </summary>
		public int Increment(int amount = 1) {
			if (amount <= 0) return 0;

			int added = Mathf.Min(this.GetSpaceLeft(), amount);
			this.count += added;
			return added;
		}
		
		/// <summary>
		/// Try to remove items. Returns how many were actually removed
		/// </summary>
		public int Decrement(int amount = 1) {
			if (amount <= 0) return 0;

			int removed = Mathf.Min(this.count, amount);
			this.count -= removed;
			return removed;
		}

		public readonly int GetSpaceLeft() => this.item.fullStackSize - this.count;

		public readonly bool IsOf(Item? item) => Equals(item, this.item);

		public static bool AreEqual(ItemStack a, ItemStack b) {
			return AreItemsEqual(a, b) && a.count == b.count;
		}

		public static bool AreItemsEqual(ItemStack a, ItemStack b) {
			return a.IsOf(b.item) && b.IsOf(a.item);
		}

		public void FillFrom(ref ItemStack itemStack) {
			if (!AreItemsEqual(itemStack, this)) return;

			int added = itemStack.Decrement(this.GetSpaceLeft());
			this.Increment(added);
		}
	
		public readonly ItemStack CopyWithCount(int newCount) {
			return new ItemStack(this.item, newCount);
		}

		public readonly ItemStack Copy() => this.CopyWithCount(this.count);

		public readonly ItemStack CopyFullStack() {
			return this.CopyWithCount(this.item.fullStackSize);
		}

		public void CapCount(int maxCount) {
			this.count = Mathf.Clamp(this.count, 0, maxCount);
		}

		[Obsolete("Cannot compare two item stacks with Equals", true)]
		public override bool Equals(object obj) {
			throw new NotSupportedException("Cannot compare two item stacks with Equals");
		}

		public readonly override int GetHashCode() {
			return HashCode.Combine(this.item, this.count);
		}

		public readonly override string ToString() {
			return $"stack[{this.item}:{this.count}]";
		}
	}
}
