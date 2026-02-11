using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class SwapTransit : SingleSlotOperation {
		public SwapTransit(IItemContainer container, int slotIndex, TransitStack transitStack)
			: base(container, slotIndex, transitStack) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && transitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			ItemStack previous = transitStack.GetStack()!;
			transitStack.SetStack(slot.GetStack()!);
			slot.SetStack(previous);
			return true;
		}
	}
}
