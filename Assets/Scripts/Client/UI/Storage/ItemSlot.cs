using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class ItemSlot : IItemSlot {
		private readonly IItemContainer container;
		private readonly int index;
		private ItemStack? stack;
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
				oldStack.onQuantityChanged -= OnQuantityChanged;
			}
			this.stack = stack;
			AssertSetStack();
			setStack?.Invoke(stack);

			if (stack != null) {
				stack.onQuantityChanged += OnQuantityChanged;
			}
			stackChanged?.Invoke(oldStack, stack);
		}

		public bool IsNullOrEmpty() {
			ItemStack? stack = GetStack();
			return stack == null || stack.IsEmpty();
		}

		private void OnQuantityChanged(int old, int @new) {
			if (stack != null) {
				quantityChanged?.Invoke(stack, old, @new);
			}
		}

		public ItemStack? GetStack() => stack;

		public int GetIndex() => index;
		public IItemContainer GetContainer() => container;

		private void AssertSetStack() {
			if (setStack == null) {
				Logger.LogError(new OperationCanceledException("SetStack is null"));
			}
		}
	}
}
