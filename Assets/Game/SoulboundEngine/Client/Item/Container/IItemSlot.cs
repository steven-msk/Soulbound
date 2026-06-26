using System;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public interface IItemSlot {
		[Obsolete]
		event Action<ItemStack> setStack;
		event Action<ItemStack, ItemStack>? stackChanged;

		ItemStack GetStack();
		void SetStack(ItemStack stack);

		int GetIndex();
		IItemContainer GetContainer();

		public bool HasStack() => !this.GetStack().IsEmpty();

		public SlotRef GetRef() => new(this.GetContainer(), this.GetIndex());
	}
}
