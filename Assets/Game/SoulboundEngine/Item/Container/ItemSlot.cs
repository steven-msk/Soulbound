#nullable enable

namespace SoulboundEngine.Item.Container {
	using System;

	public class ItemSlot : IItemSlot {
		private readonly IInventory inventory;
		private readonly int index;
		private ItemStack stack;
		[Obsolete] public event Action<ItemStack>? setStack;
		public event Action<ItemStack, ItemStack>? stackChanged;

		public ItemSlot(IInventory inventory, int index) {
			this.inventory = inventory;
			this.index = index;
		}

		public virtual void SetStack(ItemStack stack) {
			if (stack.IsEmpty()) stack = ItemStack.EMPTY;
			ItemStack oldStack = this.stack;
			this.stack = stack;
			setStack?.Invoke(stack);
			stackChanged?.Invoke(oldStack, stack);
		}

		public bool IsEmpty() {
			ItemStack stack = this.GetStack();
			return stack.IsEmpty();
		}

		public ItemStack GetStack() => this.stack;

		public int GetIndex() => this.index;
		public IInventory GetInventory() => this.inventory;
	}
}
