using System;
using System.Collections.Generic;

namespace SoulboundEngine.Item.Container {
	public readonly struct SlotRef : IEquatable<SlotRef> {
		public readonly IInventory inventory;
		public readonly int index;

		public SlotRef(IInventory container, int index) {
			this.inventory = container;
			this.index = index;
		}

		public IItemSlot GetSlot() => this.inventory.GetSlot(this.index);

		public override string ToString() {
			return $"slot[{this.inventory}/{this.index}]";
		}

		public bool Equals(SlotRef other) {
			return ReferenceEquals(this.inventory, other.inventory)
				&& this.index == other.index;
		}

		public override int GetHashCode() {
			return HashCode.Combine(this.inventory, this.index);
		}

		public sealed class Comparer : IComparer<SlotRef> {
			public int Compare(SlotRef x, SlotRef y) {
				if (!ReferenceEquals(x.inventory, y.inventory)) {
					return x.inventory.GetHashCode().CompareTo(y.inventory.GetHashCode());
				}
				return x.index.CompareTo(y.index);
			}
		}

		public sealed class EqualityComparer : IEqualityComparer<SlotRef> {
			public bool Equals(SlotRef x, SlotRef y) {
				return x.Equals(y);
			}

			public int GetHashCode(SlotRef obj) => obj.GetHashCode();
		}
	}
}
