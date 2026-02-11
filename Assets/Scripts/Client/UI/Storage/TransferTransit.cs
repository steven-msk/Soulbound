using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class TransferTransit : SingleSlotOperation {
		private readonly IItemContainer container;
		private readonly int slotIndex;

		public TransferTransit(IItemContainer container, int slotIndex, TransitStack transitStack)
			: base(container, slotIndex, transitStack) {
			this.container = container;
			this.slotIndex = slotIndex;
		}

		public override bool CanExecute() {
			return slot.HasStack() || transitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			MergeTransitInSlot mergeInSlot = new(container, slotIndex, transitStack);
			if (mergeInSlot.CanExecute()) return mergeInSlot.Execute();

			GrabStackFromSlot grabFromSlot = new(container, slotIndex, transitStack);
			if (grabFromSlot.CanExecute()) return grabFromSlot.Execute();

			SwapTransit swapTransit = new(container, slotIndex, transitStack);
			if (swapTransit.CanExecute()) return swapTransit.Execute();

			return false;
		}
	}
}
