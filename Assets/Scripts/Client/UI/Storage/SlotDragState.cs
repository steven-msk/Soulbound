using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.UI {
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

		public bool TryGetQuantity(IItemContainer container, int slotIndex, out int quantity) {
			SlotRef slotRef = new(container, slotIndex);
			return quantitySnapshots.TryGetValue(slotRef, out quantity);
		}
	}
}
