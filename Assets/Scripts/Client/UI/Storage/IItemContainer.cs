using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public interface IItemContainer {
		[Obsolete] public IReadOnlyList<IItemSlot> slots { get; }
		[Obsolete] public Transform? transform { get; }

		public IReadOnlyList<IItemSlot> GetAllSlots();

		[Obsolete] void OnPointerDown(IItemSlot slot, PointerEventData eventData);
		[Obsolete] void OnPointerUp(IItemSlot slot, PointerEventData eventData);
		[Obsolete] void OnPointerEnter(IItemSlot slot, PointerEventData data);
		[Obsolete] void OnPointerExit(IItemSlot slot, PointerEventData data);

		[Obsolete] void OnItemDisplayAdded(ItemDisplay itemDisplay, IItemSlot slot);

		public bool ContainsItem(Item item) {
			return GetAllSlots().Any(s => s.GetStack()?.item == item);
		}

	}
}
