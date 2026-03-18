using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class TransferTransit : SingleSlotOperation {
		private readonly IItemContainer container;
		private readonly int slotIndex;

		public TransferTransit(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
			this.container = container;
			this.slotIndex = slotIndex;
		}

		public override bool CanExecute() {
			return slot.HasStack() || scope.HasTransitStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			MergeTransitInSlot mergeInSlot = new(container, slotIndex, scope);
			if (mergeInSlot.CanExecute()) return mergeInSlot.Execute();

			GrabStackFromSlot grabFromSlot = new(container, slotIndex, scope);
			if (grabFromSlot.CanExecute()) return grabFromSlot.Execute();

			SwapTransit swapTransit = new(container, slotIndex, scope);
			if (swapTransit.CanExecute()) return swapTransit.Execute();

			return false;
		}
	}
}
