using SoulboundBackend.Client.UI.Storage;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public abstract class SingleSlotOperation : ISlotOperation {
		protected readonly IItemSlot slot;

		public SingleSlotOperation(IItemContainer container, int slotIndex) {
			this.slot = container.GetSlot(slotIndex);
		}

		public abstract bool CanExecute();
		public abstract bool Execute();
	}
}
