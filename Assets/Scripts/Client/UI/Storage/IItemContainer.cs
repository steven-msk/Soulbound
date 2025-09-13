using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.UI.Storage {
	public interface IItemContainer {
		public IReadOnlyList<IItemSlot> slots { get; }

		public IItemSlot GetSlotByIndex(int index);

		void OnPointerDown(IItemSlot slot, PointerEventData eventData);
		void OnPointerUp(IItemSlot slot, PointerEventData eventData);
		void OnPointerEnter(IItemSlot slot, PointerEventData data);
		void OnPointerExit(IItemSlot slot, PointerEventData data);
	}
}
