using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class ReleaseTransitInEmptySlot : SingleSlotOperation {
		public ReleaseTransitInEmptySlot(IItemContainer container, int slotIndex, TransitStack transitStack)
			: base(container, slotIndex, transitStack) {
		}

		public override bool CanExecute() {
			return !slot.HasStack() && transitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			slot.SetStack(transitStack.GetStack());
			transitStack.Release();
			return true;
		}
	}
}
