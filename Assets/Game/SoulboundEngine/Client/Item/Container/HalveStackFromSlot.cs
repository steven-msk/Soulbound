namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class HalveStackFromSlot : SingleSlotOperation {
		public HalveStackFromSlot(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && !scope.HasTransitStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			int half = slot.GetStack().quantity / 2;
			int remainder = slot.GetStack().quantity % 2;
			int transfer = half + remainder;

			ItemStack halvedTransit = slot.GetStack().Clone(transfer);
			slot.GetStack().Decrement(transfer);
			scope.SetTransitStack(halvedTransit);
			return true;
		}
	}
}
