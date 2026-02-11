using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class HalveStackFromSlot : SingleSlotOperation {
		public HalveStackFromSlot(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && !scope.transitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;


			int half = slot.GetStack()!.quantity / 2;
			int remainder = slot.GetStack()!.quantity % 2;
			int transfer = half + remainder;
			slot.GetStack()!.Decrement(transfer);
			ItemStack halvedTransit = new(slot.GetStack()!.item, transfer);
			scope.transitStack.SetStack(halvedTransit);
			return true;
		}
	}
}
