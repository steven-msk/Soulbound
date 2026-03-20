

#nullable enable

using SoulboundBackend.Client.Debug.Logging;

namespace SoulboundBackend.Client.ItemSystem.Container {
	public abstract class SingleSlotOperation : ISlotOperation {
		protected readonly IItemSlot slot;
		protected readonly IItemContainerScope scope;

		public SingleSlotOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			this.slot = container.GetSlot(slotIndex);
			this.scope = scope;
		}

		public abstract bool CanExecute();
		public abstract bool Execute();
	}
}
