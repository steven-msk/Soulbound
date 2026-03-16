using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public sealed class SwapTransit : SingleSlotOperation {
		public SwapTransit(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
		}

		public override bool CanExecute() {
			return slot.HasStack() && scope.HasTransitStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			ItemStack previousTransit = scope.GetTransitStack();
			scope.SetTransitStack(slot.GetStack());
			slot.SetStack(previousTransit);
			return true;
		}
	}
}
