namespace SoulboundEngine.Item.Container {
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public static class InventoryUtils {
		/// <summary>
		/// Tries to add the stack into the inventory.
		/// Returns whether the stack was fully consumed.
		/// </summary>
		public static bool TryAddStack(this IInventory inventory, ref ItemStack itemStack) {
			return TryAddStack(inventory.GetAllSlots(), ref itemStack);
		}

		/// <summary>
		/// Tries to add the stack into the slots.
		/// Returns whether the stack was fully consumed.
		/// </summary>
		public static bool TryAddStack(IEnumerable<IItemSlot> slots, ref ItemStack itemStack) {
			List<IItemSlot> slotList = slots.ToList();
			foreach (IItemSlot slot in FilterContaining(slotList, itemStack.GetItem())) {
				ItemStack slotStack = slot.GetStack();
				slotStack.FillFrom(ref itemStack);
				slot.SetStack(slotStack);
				if (itemStack.IsEmpty()) return true;
			}
			if (TryGetFirstEmpty(slotList, out IItemSlot emptySlot)) {
				emptySlot.SetStack(itemStack);
				itemStack = ItemStack.EMPTY;
				return true;
			}
			return false;
		}

		public static IEnumerable<IItemSlot> GetSlotsContaining(this IInventory inventory, Item? item) {
			foreach (IItemSlot slot in inventory.GetAllSlots()) {
				if (slot.GetStack().IsOf(item)) {
					yield return slot;
				}
			}
		}

		public static IEnumerable<IItemSlot> FilterContaining(IEnumerable<IItemSlot> slots, Item? item) {
			List<IItemSlot> filtered = new();
			foreach (IItemSlot slot in slots) {
				if (slot.GetStack().IsOf(item)) {
					filtered.Add(slot);
				}
			}
			return filtered;
		}

		public static bool TryGetFirstEmpty(IEnumerable<IItemSlot> slots, out IItemSlot empty) {
			foreach (IItemSlot slot in slots) {
				if (!slot.HasStack()) {
					empty = slot;
					return true;
				}
			}
			empty = default!;
			return false;
		}

		public static bool ContainsItem(this IInventory inventory, Item? item) {
			foreach (IItemSlot _ in inventory.GetSlotsContaining(item)) {
				return true;
			}
			return false;
		}

		public static bool TryGetFirstEmptySlot(this IInventory inventory, out IItemSlot slot) {
			foreach (IItemSlot emptySlot in inventory.GetSlotsContaining(null)) {
				slot = emptySlot;
				return true;
			}
			slot = null!;
			return false;
		}

		public static IEnumerable<IItemSlot> GetFreeSlots(this IInventory inventory) {
			return inventory.GetSlotsContaining(null);
		}

		public static void Clear(this IInventory inventory) {
			inventory.Clear();
		}
	}
}
