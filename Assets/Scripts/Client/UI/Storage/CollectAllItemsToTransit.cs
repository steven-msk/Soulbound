using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System.Linq;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class CollectAllItemsToTransit : SingleSlotOperation {
		private readonly Item item;
		private readonly IItemContainer container;

		public CollectAllItemsToTransit(Item item, IItemContainer container, int slotIndex, TransitStack transitStack)
			: base(container, slotIndex, transitStack) {
			this.item = item;
			this.container = container;
		}

		public override bool CanExecute() {
			if (!transitStack.HasStack()) return false;
			return item != null && !transitStack.GetStack()!.IsFull();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			var slots = container.GetSlotsContaining(item)
			.OrderBy(slot => slot.GetStack()!.quantity)
			.ToList();
			if (slots == null || slots.Count == 0) return false;

			ItemStack transitStack = this.transitStack.GetStack()!;
			int spaceLeft = item.maxStackSize - transitStack.quantity;
			foreach (var slot in slots) {
				if (spaceLeft <= 0) break;

				int transfer = transitStack.Increment(slot.GetStack()!.quantity);
				slot.GetStack()!.Decrement(transfer);
				spaceLeft -= transfer;
			}
			return true;
		}
	}
}
