using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.UI {
	public abstract class SingleDraggedSlotOperation : SingleSlotOperation {
		protected readonly SlotDragContext dragCtx;

		protected SingleDraggedSlotOperation(IItemContainer container, int slotIndex, SlotDragContext dragCtx)
			: base(container, slotIndex) {
			this.dragCtx = dragCtx;
		}
	}
}
