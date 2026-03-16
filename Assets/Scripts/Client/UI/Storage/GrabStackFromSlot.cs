using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class GrabStackFromSlot : SingleSlotOperation {
		public GrabStackFromSlot(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && !scope.HasTransitStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			scope.SetTransitStack(slot.GetStack());
			slot.SetStack(null);
			return true;
		}
	}
}
