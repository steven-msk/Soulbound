using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public interface IItemContainer : IItemContainerDomain {
		[Obsolete] public IReadOnlyList<IItemSlot> slots { get; }

		IReadOnlyList<int> GetAllSlots();
		int GetSlotCount();

		[Obsolete] void OnItemDisplayAdded(ItemDisplay itemDisplay, IItemSlot slot);

		public bool ContainsItem(Item item) {
			return GetAllSlots().Any(i => GetSlot(i).GetStack()?.item == item);
		}

		public IEnumerable<IItemSlot> GetSlotsContaining(Item item) {
			foreach (var slotIndex in GetAllSlots()) {
				IItemSlot slot = GetSlot(slotIndex);

				if (slot.GetStack()?.item == item) {
					yield return slot;
				}
			}
		}
	}
}
