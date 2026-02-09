using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class GrabStackFromSlot : SingleSlotOperation {
		public GrabStackFromSlot(IItemContainer container, int slotIndex)
			: base(container, slotIndex) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && !TransitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			TransitStack.instance.SetStack(slot.GetStack()!);
			slot.SetStack(null);
			return true;
		}
	}
}
