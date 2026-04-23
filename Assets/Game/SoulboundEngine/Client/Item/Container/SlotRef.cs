
using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.ItemSystem.Container {
	public readonly struct SlotRef : IEquatable<SlotRef> {
		public readonly IItemContainer container;
		public readonly int index;

		public SlotRef(IItemContainer container, int index) {
			this.container = container;
			this.index = index;
		}

		public IItemSlot GetSlot() => container.GetSlot(index);

		public bool Equals(SlotRef other) {
			return ReferenceEquals(container, other.container)
				&& index == other.index;
		}

		public override int GetHashCode() {
			return HashCode.Combine(container, index);
		}

		public sealed class Comparer : IComparer<SlotRef> {
			public int Compare(SlotRef x, SlotRef y) {
				if (!ReferenceEquals(x.container, y.container)) {
					return x.container.GetHashCode().CompareTo(y.container.GetHashCode());
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
