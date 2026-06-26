using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public static class ItemContainerUtils {
		public static bool TryAddStack(this IItemContainer container, ItemStack itemStack) {
			foreach (var slot in container.GetSlotsContaining(itemStack.item)) {
				ItemStack slotStack = slot.GetStack();
				slotStack.FillFrom(ref itemStack);
				slot.SetStack(slotStack);
				if (itemStack.IsEmpty()) return true;
			}
			if (TryGetFirstEmptySlot(container, out IItemSlot emptySlot)) {
				emptySlot.SetStack(itemStack);
				return true;
			}
			return false;
		}

		public static IEnumerable<IItemSlot> GetSlotsContaining(this IItemContainer container, Item? item) {
			foreach (var slotIndex in container.GetAllSlots()) {
				IItemSlot slot = container.GetSlot(slotIndex);

				if (slot.GetStack().IsOf(item!)) {
					yield return slot;
				}
			}
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
