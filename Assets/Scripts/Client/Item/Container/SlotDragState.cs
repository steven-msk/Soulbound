using SoulboundBackend.Client.ItemSystem;

using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.ItemSystem.Container {
	public sealed class SlotDragState {
		private readonly IItemContainer originContainer;
		public Item item { get; init; }
		public SlotRef origin { get; init; }
		public SortedSet<SlotRef> draggedSlots { get; init; }
		public PointerEventData.InputButton button { get; init; }
		public Dictionary<SlotRef, int> quantitySnapshots { get; init; }
		public int originStack { get; init; }

		public SlotDragState(IItemContainer originContainer) {
			this.originContainer = originContainer;
		}

		public IItemContainer GetOriginContainer() => originContainer;

		public void AddDraggedSlot(IItemContainer container, int slotIndex) {
			draggedSlots.Add(new SlotRef(container, slotIndex));
		}

		public bool IsSlotDragged(SlotRef slotRef) => draggedSlots.Contains(slotRef);
	}
}
