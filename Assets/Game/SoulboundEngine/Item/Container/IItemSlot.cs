namespace SoulboundEngine.Item.Container {
	using System;

#nullable enable

	public interface IItemSlot {
		event Action<ItemStack, ItemStack>? stackChanged;

		ItemStack GetStack();
		void SetStack(ItemStack stack);

		int GetIndex();
		IInventory GetInventory();
	}

	public static class ItemSlotDefaults {
		public static SlotRef GetRef(this IItemSlot slot) {
			return new SlotRef(slot.GetInventory(), slot.GetIndex());
		}

		public static bool HasStack(this IItemSlot slot) => !slot.GetStack().IsEmpty();

		public static bool IsEmpty(this IItemSlot slot) => slot.GetStack().IsEmpty();
	}
}
