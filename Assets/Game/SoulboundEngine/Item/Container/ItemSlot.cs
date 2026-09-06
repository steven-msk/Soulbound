namespace SoulboundEngine.Item.Container {
	using System;

#nullable enable

	public class ItemSlot : IItemSlot {
		private readonly IInventory inventory;
		private readonly int index;
		private ItemStack stack;
		public event Action<ItemStack, ItemStack>? stackChanged;

		public ItemSlot(IInventory inventory, int index) {
			this.inventory = inventory;
			this.index = index;
		}

		public virtual void SetStack(ItemStack stack) {
			if (stack.IsEmpty()) stack = ItemStack.EMPTY;
			ItemStack oldStack = this.stack;
			this.stack = stack;
			stackChanged?.Invoke(oldStack, stack);
		}

		public ItemStack GetStack() => this.stack;

		public int GetIndex() => this.index;
		public IInventory GetInventory() => this.inventory;
	}
}
