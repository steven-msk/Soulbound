using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public interface IItemContainer {
		[Obsolete] public IReadOnlyList<IItemSlot> slots { get; }
		[Obsolete] public Transform? transform { get; }

		public IItemSlot GetSlotByIndex(int index);
		public IReadOnlyList<IItemSlot> GetAllSlots();

		[Obsolete] void OnPointerDown(IItemSlot slot, PointerEventData eventData);
		[Obsolete] void OnPointerUp(IItemSlot slot, PointerEventData eventData);
		[Obsolete] void OnPointerEnter(IItemSlot slot, PointerEventData data);
		[Obsolete] void OnPointerExit(IItemSlot slot, PointerEventData data);

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
