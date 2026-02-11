using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public interface IItemContainerScope {
		TransitStack transitStack { get; }

		bool InDragState();
		SlotDragState? GetDragState();

		bool TryBeginDrag(IItemContainer container, int originSlotIndex, PointerEventData.InputButton button);
		void ExtendDrag(IItemContainer container, int slotIndex);
		void EndDrag();
	}
}
