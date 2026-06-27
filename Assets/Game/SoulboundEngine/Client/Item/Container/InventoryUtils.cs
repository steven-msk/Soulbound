using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public static class InventoryUtils {
		public static bool TryAddStack(this IItemContainer container, ref ItemStack itemStack) {
			return TryAddStack(container.GetAllSlots().Select(i => container.GetSlot(i)), ref itemStack);
		}

		public static bool TryAddStack(IEnumerable<IItemSlot> slots, ref ItemStack itemStack) {
			foreach (var slot in FilterContaining(slots, itemStack.item)) {
				ItemStack slotStack = slot.GetStack();
				slotStack.FillFrom(ref itemStack);
				slot.SetStack(slotStack);
				if (itemStack.IsEmpty()) return true;
			}
			if (TryGetFirstEmpty(slots, out IItemSlot emptySlot)) {
				emptySlot.SetStack(itemStack);
				itemStack = ItemStack.EMPTY;
				return true;
			}
			return false;
		}

		public static IEnumerable<IItemSlot> GetSlotsContaining(this IItemContainer container, Item? item) {
			foreach (var slotIndex in container.GetAllSlots()) {
				IItemSlot slot = container.GetSlot(slotIndex);

				if (slot.GetStack().IsOf(item)) {
					yield return slot;
				}
			}
		}

		public static IEnumerable<IItemSlot> FilterContaining(IEnumerable<IItemSlot> slots, Item? item) {
			List<IItemSlot> filtered = new();
			foreach (var slot in slots) {
				if (slot.GetStack().IsOf(item)) {
					filtered.Add(slot);
				}
			}
			return filtered;
		}

		public static bool TryGetFirstEmpty(IEnumerable<IItemSlot> slots, out IItemSlot empty) {
			foreach (var slot in slots) {
				if (!slot.HasStack()) {
					empty = slot;
					return true;
				}
			}
			empty = default!;
			return false;
		}

		public static bool ContainsItem(this IItemContainer container, Item? item) {
			foreach (var _ in container.GetSlotsContaining(item)) {
				return true;
			}
			return false;
		}

		public static bool TryGetFirstEmptySlot(this IItemContainer container, out IItemSlot slot) {
			foreach (var emptySlot in container.GetSlotsContaining(null)) {
				slot = emptySlot;
				return true;
			}
			slot = null!;
			return false;
		}
	}
}
