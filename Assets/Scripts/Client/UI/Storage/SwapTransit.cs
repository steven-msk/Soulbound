using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class SwapTransit : SingleSlotOperation {
		public SwapTransit(IItemContainer container, int slotIndex)
			: base(container, slotIndex) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && TransitStack.HasStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			ItemStack previous = TransitStack.GetStack()!;
			TransitStack.instance.SetStack(slot.GetStack()!);
			slot.SetStack(previous);
			return true;
		}
	}
}
