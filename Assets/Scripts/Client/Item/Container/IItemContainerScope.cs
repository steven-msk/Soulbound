using SoulboundBackend.Client.ItemSystem.Container;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem.Container {
	public interface IItemContainerScope : ITransitStackSource {
		IEnumerable<IItemContainer> GetOpenContainers();

		bool InDragState();
		SlotDragState? GetDragState();

		bool TryBeginDrag(ItemStack stack, SlotRef slotRef, PointerEventData.InputButton button);
		void ExtendDrag(SlotRef slotRef);
		void EndDrag();
	}
}
