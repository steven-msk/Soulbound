using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class HalveStackFromSlot : SingleSlotOperation {
		public HalveStackFromSlot(IItemContainer container, int slotIndex, TransitStack transitStack)
			: base(container, slotIndex, transitStack) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && !transitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;


			int half = slot.GetStack()!.quantity / 2;
			int remainder = slot.GetStack()!.quantity % 2;
			int transfer = half + remainder;
			slot.GetStack()!.Decrement(transfer);
			transitStack.SetStack(new ItemStack(slot.GetStack()!.item, transfer));
			return true;
		}
	}
}
