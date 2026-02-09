using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class MergeTransitInSlot : SingleSlotOperation {
		private readonly IItemContainer container;
		private readonly int slotIndex;

		public MergeTransitInSlot(IItemContainer container, int slotIndex)
			: base(container, slotIndex) {
			this.container = container;
			this.slotIndex = slotIndex;
		}

		public override bool CanExecute() => TransitStack.HasStack();

		public override bool Execute() {
			ReleaseTransitInEmptySlot releaseInSlot = new(container, slotIndex);
			if (releaseInSlot.CanExecute()) return releaseInSlot.Execute();

			if (!CanExecute()) return false;

			ItemStack transitStack = TransitStack.GetStack()!;
			int space = transitStack.item.maxStackSize - slot.GetStack()!.quantity;
			if (space <= 0) return false;

			int transfer = Math.Min(space, transitStack.quantity);
			slot.GetStack()!.Increment(transfer);
			transitStack.Decrement(transfer);
			return true;
		}
	}
}
