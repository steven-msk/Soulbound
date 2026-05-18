

#nullable enable

using SoulboundEngine.Client.Debug.Logging;

namespace SoulboundEngine.Client.ItemSystem.Container {
	public abstract class SingleSlotOperation : ISlotOperation {
		protected readonly IItemSlot slot;
		protected readonly IInventoryScope scope;

		public SingleSlotOperation(IItemContainer container, int slotIndex, IInventoryScope scope) {
			this.slot = container.GetSlot(slotIndex);
			this.scope = scope;
		}

		public abstract bool CanExecute();
		public abstract bool Execute();
	}
}
