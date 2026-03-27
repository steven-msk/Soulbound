using SoulboundEngine.Client.ItemSystem;

using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class SlotDragState {
		private readonly IItemContainer originContainer;
		public ItemStack stack { get; init; }
		public SlotRef origin { get; init; }
		public HashSet<SlotRef> draggedSlots { get; init; }
		public PointerEventData.InputButton button { get; init; }
		public Dictionary<SlotRef, int> quantitySnapshots { get; init; }

		public SlotDragState(IItemContainer originContainer) {
			this.originContainer = originContainer;
		}

		public IItemContainer GetOriginContainer() => originContainer;

		public void ExtendDrag(SlotRef slotRef) {
			draggedSlots.Add(slotRef);
		}

		public bool IsSlotDragged(SlotRef slotRef) => draggedSlots.Contains(slotRef);
	}
}
