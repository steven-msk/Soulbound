#nullable enable

using SoulboundBackend.Client.ItemSystem;
using System;

namespace SoulboundBackend.Client.UI.Storage {
	[Obsolete]
	public class GrabbedItemContext {
		public ItemDisplay? value { get; private set; }
		public IItemSlot? lastKnownSlot { get; private set; }

		public GrabbedItemContext(ItemDisplay? initialValue, IItemSlot? currentSlot) {
			this.value = initialValue;
			this.lastKnownSlot = currentSlot;
		}

		public void Set(ItemDisplay? value, IItemSlot? slot) {
			this.value = value;
			if (slot != null) {
				this.lastKnownSlot = slot;
			}
		}
	}
}
