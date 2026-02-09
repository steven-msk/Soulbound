using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System.Linq;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class CollectAllItemsToTransit : SingleSlotOperation {
		private readonly Item item;
		private readonly IItemContainer container;

		public CollectAllItemsToTransit(Item item, IItemContainer container, int slotIndex)
			: base(container, slotIndex) {
			this.item = item;
			this.container = container;
		}

		public override bool CanExecute() {
			if (!TransitStack.HasStack()) return false;
			return item != null && !TransitStack.GetStack()!.IsFull();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			var slots = container.GetSlotsContaining(item)
			.OrderBy(slot => slot.GetStack()!.quantity)
			.ToList();
			if (slots == null || slots.Count == 0) return false;

			ItemStack transitStack = TransitStack.GetStack()!;
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
