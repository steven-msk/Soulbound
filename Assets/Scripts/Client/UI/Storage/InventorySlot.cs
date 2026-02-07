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
	public sealed class InventorySlot : IItemSlot {
		public int index { get; [Obsolete] set; }
		public IItemContainer container { get; }
		private ItemStack? stack;
		public event Action<ItemStack> setStack;

		public InventorySlot(IItemContainer container, int index) {
			this.container = container;
			this.index = index;
		}

		[Obsolete] public ItemDisplay itemDisplay => throw new NotImplementedException();

		[Obsolete] public Transform transform => throw new NotImplementedException();

		[Obsolete] public bool showTooltip { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Deserialize(SerializedItemSlot serialized) {
			stack = serialized.itemStack;
		}

		public int GetIndex() => index;
		public ItemStack? GetStack() => stack;

		public void SetStack(ItemStack stack) {
			this.stack = stack;
			setStack(stack);

		}
	}
}
