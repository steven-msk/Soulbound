using System;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public class ItemSlot : IItemSlot {
		private readonly IItemContainer container;
		private readonly int index;
		private ItemStack stack;
		[Obsolete]
		public event Action<ItemStack>? setStack;
		public event Action<ItemStack, ItemStack>? stackChanged;

		public ItemSlot(IItemContainer container, int index) {
			this.container = container;
			this.index = index;
		}

		public void SetStack(ItemStack stack) {
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
		public IItemContainer GetContainer() => this.container;
	}
}
