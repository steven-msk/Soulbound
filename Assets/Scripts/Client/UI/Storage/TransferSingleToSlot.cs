using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class TransferSingleToSlot : SingleSlotOperation {
		public TransferSingleToSlot(IItemContainer container, int slotIndex, TransitStack transitStack)
			: base(container, slotIndex, transitStack) {
		}

		public override bool CanExecute() => transitStack.HasStack();

		public override bool Execute() {
			if (!CanExecute()) return false;

			ItemStack transitStack = this.transitStack.GetStack()!;
			if (!slot.HasStack()) slot.SetStack(new ItemStack(transitStack.item, 0));

			int added = slot.GetStack()!.Increment();
			if (added > 0) transitStack.Decrement();
			return true;
		}
	}
}
