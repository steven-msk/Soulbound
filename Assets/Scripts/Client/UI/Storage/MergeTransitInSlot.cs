using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class MergeTransitInSlot : SingleSlotOperation {
		private readonly IItemContainer container;
		private readonly int slotIndex;

		public MergeTransitInSlot(IItemContainer container, int slotIndex, TransitStack transitStack)
			: base(container, slotIndex, transitStack) {
			this.container = container;
			this.slotIndex = slotIndex;
		}

		public override bool CanExecute() => transitStack.HasStack();

		public override bool Execute() {
			ReleaseTransitInEmptySlot releaseInSlot = new(container, slotIndex, this.transitStack);
			if (releaseInSlot.CanExecute()) return releaseInSlot.Execute();

			if (!CanExecute()) return false;

			ItemStack transitStack = this.transitStack.GetStack()!;
			int space = transitStack.item.maxStackSize - slot.GetStack()!.quantity;
			if (space <= 0) return false;

			int transfer = Math.Min(space, transitStack.quantity);
			slot.GetStack()!.Increment(transfer);
			transitStack.Decrement(transfer);
			return true;
		}
	}
}
