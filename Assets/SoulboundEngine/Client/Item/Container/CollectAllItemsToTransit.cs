using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class CollectAllItemsToTransit : ISlotOperation {
		private readonly IItemContainerScope scope;

		public CollectAllItemsToTransit(IItemContainerScope scope) {
			this.scope = scope;
		}

		public bool CanExecute() {
			if (!scope.HasTransitStack()) return false;
			return !scope.GetTransitStack()!.IsFull();
		}

		private List<IItemSlot> GetSlotsContaining(Item item) {
			return scope.GetOpenContainers()
				.SelectMany(c => c.GetSlotsContaining(item))
				.OrderBy(s => s.GetStack()!.quantity)
				.ToList();
		}

		bool ISlotOperation.Execute() {
			if (!CanExecute()) return false;

			ItemStack transitStack = scope.GetTransitStack()!;
			Item item = transitStack.item;
			List<IItemSlot> slots = GetSlotsContaining(item);
			if (slots == null || slots.Count == 0) return false;

			foreach (var slot in slots) {
				transitStack.FillFrom(slot.GetStack());
			}
			return true;
		}
	}
}
