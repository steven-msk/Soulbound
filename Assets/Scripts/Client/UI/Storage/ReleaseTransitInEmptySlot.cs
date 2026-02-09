using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class ReleaseTransitInEmptySlot : SingleSlotOperation {
		public ReleaseTransitInEmptySlot(IItemContainer container, int slotIndex)
			: base(container, slotIndex) {
		}

		public override bool CanExecute() {
			return !slot.HasStack() && TransitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			slot.SetStack(TransitStack.GetStack());
			TransitStack.instance.Release();
			return true;
		}
	}
}
