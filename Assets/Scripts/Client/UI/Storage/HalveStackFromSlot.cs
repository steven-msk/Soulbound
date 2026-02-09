using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class HalveStackFromSlot : SingleSlotOperation {
		public HalveStackFromSlot(IItemContainer container, int slotIndex)
			: base(container, slotIndex) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && !TransitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;


			int half = slot.GetStack()!.quantity / 2;
			int remainder = slot.GetStack()!.quantity % 2;
			int transfer = half + remainder;
			slot.GetStack()!.Decrement(transfer);
			TransitStack.instance.SetStack(new ItemStack(slot.GetStack()!.item, transfer));
			return true;
		}
	}
}
