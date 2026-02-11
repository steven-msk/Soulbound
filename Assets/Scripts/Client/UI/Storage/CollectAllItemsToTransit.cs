using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class CollectAllItemsToTransit : ISlotOperation {
		private readonly IItemContainerScope scope;

		public CollectAllItemsToTransit(IItemContainerScope scope) {
			this.scope = scope;
		}

		public bool CanExecute() {
			if (!scope.transitStack.HasStack()) return false;
			return !scope.transitStack.GetStack()!.IsFull();
		}

		private List<IItemSlot> GetSlotsContaining(Item item) {
			return scope.GetOpenContainers()
				.SelectMany(c => c.GetSlotsContaining(item))
				.OrderBy(s => s.GetStack()!.quantity)
				.ToList();
		}

		bool ISlotOperation.Execute() {
			if (!CanExecute()) return false;

			Item item = scope.transitStack.GetStack()!.item;
			List<IItemSlot> slots = GetSlotsContaining(item);
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
