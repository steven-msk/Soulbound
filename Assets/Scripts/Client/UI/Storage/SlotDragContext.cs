using SoulboundBackend.Client.ItemSystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public struct SlotDragContext {
		public Item item;
		public int origin;
		public HashSet<int> draggedSlots;
		public PointerEventData.InputButton button;
		public Dictionary<int, int> quantitySnapshots;
		public int originStack;
	}
}
