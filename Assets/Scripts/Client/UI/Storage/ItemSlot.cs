using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class ItemSlot : IItemSlot {
		private readonly IItemContainer container;
		private readonly int index;
		private ItemStack? stack;
		public event Action<ItemStack?>? setStack;

		public ItemSlot(IItemContainer container, int index) {
			this.container = container;
			this.index = index;
		}

		public ItemStack? GetStack() => stack;

		public void SetStack(ItemStack? stack) {
			this.stack = stack;
			AssertSetStack();
			setStack?.Invoke(stack);
		}

		private void AssertSetStack() {
			if (setStack == null) {
				UnityEngine.Debug.LogException(new InvalidOperationException("SetStack callback is null"));
			}
		}

		public int GetIndex() => index;
		public IItemContainer GetContainer() => container;
	}
}
