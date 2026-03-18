using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System;

namespace SoulboundBackend.Client.UI {
	public sealed class MergeTransitInSlot : SingleSlotOperation {
		private readonly IItemContainer container;
		private readonly int slotIndex;

		public MergeTransitInSlot(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
			this.container = container;
			this.slotIndex = slotIndex;
		}

		public override bool CanExecute() {
			return slot.HasStack()
				? slot.GetStack().IsStackableWith(scope.GetTransitStack())
				: scope.HasTransitStack();
		}

		public override bool Execute() {
			ReleaseTransitInEmptySlot releaseInSlot = new(container, slotIndex, scope);
			if (releaseInSlot.CanExecute()) return releaseInSlot.Execute();

			if (!CanExecute()) return false;

			int space = slot.GetStack().GetSpaceLeft();
			if (space <= 0) return false;

			ItemStack transitStack = scope.GetTransitStack();
			int transfer = Math.Min(space, transitStack.quantity);

			slot.GetStack().Increment(transfer);
			transitStack.Decrement(transfer);

			return true;
		}
	}
}
