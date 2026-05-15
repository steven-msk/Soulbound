using System.Collections.Generic;

namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class SlotDragState {
		private readonly IItemContainer originContainer;
		public ItemStack stack { get; init; }
		public SlotRef origin { get; init; }
		public HashSet<SlotRef> draggedSlots { get; init; }
		public int button { get; init; }
		public Dictionary<SlotRef, int> quantitySnapshots { get; init; }

		public SlotDragState(IItemContainer originContainer) {
			this.originContainer = originContainer;
		}

		public IItemContainer GetOriginContainer() => this.originContainer;

		public void ExtendDrag(SlotRef slotRef) {
			this.draggedSlots.Add(slotRef);
		}

		public bool IsSlotDragged(SlotRef slotRef) => this.draggedSlots.Contains(slotRef);
	}
}
