using SoulboundEngine.Client.ItemSystem;


namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class TransferSingleToSlot : SingleSlotOperation {
		public TransferSingleToSlot(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
		}

		public override bool CanExecute() {
			return slot.HasStack()
				? slot.GetStack().IsStackableWith(scope.GetTransitStack())
				: scope.HasTransitStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			ItemStack transitStack = scope.GetTransitStack();
			if (!slot.HasStack()) {
				ItemStack cloned = transitStack.Clone(1);
				transitStack.Decrement();
				slot.SetStack(cloned);
				return true;
			}

			int added = slot.GetStack().Increment();
			if (added > 0) transitStack.Decrement();
			return true;
		}
	}
}
