using SoulboundBackend.Client.ItemSystem.Container;
using System.Collections.Generic;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem.Container {
	public interface IItemContainerScope : ITransitStackSource {
		IEnumerable<IItemContainer> GetOpenContainers();

		bool InDragState();
		SlotDragState? GetDragState();

		bool TryBeginDrag(IItemContainer container, int originSlotIndex, PointerEventData.InputButton button);
		void ExtendDrag(IItemContainer container, int slotIndex);
		void EndDrag();
	}
}
