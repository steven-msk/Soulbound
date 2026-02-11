using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.UI {
	public record SlotDragState {
		private readonly IItemContainer originContainer;
		public Item item { get; init; }
		public int origin { get; init; }
		public HashSet<int> draggedSlots { get; init; }
		public PointerEventData.InputButton button { get; init; }
		public Dictionary<int, int> quantitySnapshots { get; init; }
		public int originStack { get; init; }

		public SlotDragState(IItemContainer originContainer) {
			this.originContainer = originContainer;
		}

		public IItemContainer GetOriginContainer() => originContainer;
	}
}
