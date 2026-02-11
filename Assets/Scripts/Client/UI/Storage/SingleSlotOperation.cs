using SoulboundBackend.Client.UI.Storage;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public abstract class SingleSlotOperation : ISlotOperation {
		protected readonly IItemSlot slot;
		protected readonly TransitStack transitStack;

		public SingleSlotOperation(IItemContainer container, int slotIndex, TransitStack transitStack) {
			this.slot = container.GetSlot(slotIndex);
			this.transitStack = transitStack;
		}

		public abstract bool CanExecute();
		public abstract bool Execute();
	}
}
