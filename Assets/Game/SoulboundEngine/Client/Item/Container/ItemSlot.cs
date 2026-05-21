using System;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public class ItemSlot : IItemSlot {
		private readonly IItemContainer container;
		private readonly int index;
		private ItemStack? stack;
		[Obsolete]
		public event Action<ItemStack?>? setStack;
		public event Action<ItemStack?, ItemStack?>? stackChanged;
		public event Action<ItemStack, int, int>? quantityChanged;

		public ItemSlot(IItemContainer container, int index) {
			this.container = container;
			this.index = index;
		}

		public void SetStack(ItemStack? stack) {
			ItemStack? oldStack = this.stack;
			if (oldStack != null) {
				oldStack.onQuantityChanged -= this.OnQuantityChanged;
			}
			this.stack = stack;
			setStack?.Invoke(stack);

			if (stack != null) {
				stack.onQuantityChanged += this.OnQuantityChanged;
			}
			stackChanged?.Invoke(oldStack, stack);
		}

		public bool IsNullOrEmpty() {
			ItemStack? stack = this.GetStack();
			return stack == null || stack.IsEmpty();
		}

		private void OnQuantityChanged(int old, int @new) {
			if (this.stack != null) {
				quantityChanged?.Invoke(this.stack, old, @new);
				if (@new <= 0) this.SetStack(null);
			}
		}

		public ItemStack? GetStack() => this.stack;

		public int GetIndex() => this.index;
		public IItemContainer GetContainer() => this.container;
	}
}
