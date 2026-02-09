using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class TransferTransit : SingleSlotOperation {
		private readonly IItemContainer container;
		private readonly int slotIndex;

		public TransferTransit(IItemContainer container, int slotIndex)
			: base(container, slotIndex) {
			this.container = container;
			this.slotIndex = slotIndex;
		}

		public override bool CanExecute() {
			return slot.HasStack() || TransitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			MergeTransitInSlot mergeInSlot = new(container, slotIndex);
			if (mergeInSlot.CanExecute()) return mergeInSlot.Execute();

			GrabStackFromSlot grabFromSlot = new(container, slotIndex);
			if (grabFromSlot.CanExecute()) return grabFromSlot.Execute();

			SwapTransit swapTransit = new(container, slotIndex);
			if (swapTransit.CanExecute()) return swapTransit.Execute();

			return false;
		}
	}
}
