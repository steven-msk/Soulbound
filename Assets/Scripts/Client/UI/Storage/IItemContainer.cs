using SoulboundBackend.Client.ItemSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public interface IItemContainer {
		public IReadOnlyList<IItemSlot> slots { get; }
		public Transform? transform { get; }
		public IItemSlot GetSlotByIndex(int index);

		void OnPointerDown(IItemSlot slot, PointerEventData eventData);
		void OnPointerUp(IItemSlot slot, PointerEventData eventData);
		void OnPointerEnter(IItemSlot slot, PointerEventData data);
		void OnPointerExit(IItemSlot slot, PointerEventData data);

		void OnItemDisplayAdded(ItemDisplay itemDisplay, IItemSlot slot);

		public virtual bool ContainsItem(Item item) {
			foreach (var slot in slots) {
				if (slot.HasItem && slot.item == item) {
					return true;
				}
			}
			return false;
		}

	}
}
