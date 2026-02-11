using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class GrabStackFromSlot : SingleSlotOperation {
		public GrabStackFromSlot(IItemContainer container, int slotIndex, TransitStack transitStack)
			: base(container, slotIndex, transitStack) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && !transitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			transitStack.SetStack(slot.GetStack()!);
			slot.SetStack(null);
			return true;
		}
	}
}
