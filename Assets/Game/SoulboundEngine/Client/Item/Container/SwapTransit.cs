using SoulboundEngine.Client.Item;


namespace SoulboundEngine.Client.Item.Container {
	public sealed class SwapTransit : SingleSlotOperation {
		public SwapTransit(IItemContainer container, int slotIndex, IInventoryScope scope)
			: base(container, slotIndex, scope) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && scope.HasTransitStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			ItemStack previousTransit = scope.GetTransitStack();
			scope.SetTransitStack(slot.GetStack());
			slot.SetStack(previousTransit);
			return true;
		}
	}
}
