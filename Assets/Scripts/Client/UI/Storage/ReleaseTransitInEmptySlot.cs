using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class ReleaseTransitInEmptySlot : SingleSlotOperation {
		public ReleaseTransitInEmptySlot(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
		}

		public override bool CanExecute() {
			return !slot.HasStack() && scope.transitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			slot.SetStack(scope.transitStack.GetStack());
			scope.transitStack.Release();
			return true;
		}
	}
}
