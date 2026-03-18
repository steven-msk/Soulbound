using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
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
	}
}
