using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class CollectAllItemsToTransit : SingleSlotOperation {
		private readonly IItemContainer container;

		public CollectAllItemsToTransit(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
			this.container = container;
		}

		public override bool CanExecute() {
			if (!scope.transitStack.HasStack()) return false;
			return !scope.transitStack.GetStack()!.IsFull();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			Item item = scope.transitStack.GetStack()!.item;
			List<IItemSlot> slots = container.GetSlotsContaining(item)
				.OrderBy(slot => slot.GetStack()!.quantity)
				.ToList();
			if (slots == null || slots.Count == 0) return false;

			ItemStack transitStack = scope.transitStack.GetStack()!;
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
