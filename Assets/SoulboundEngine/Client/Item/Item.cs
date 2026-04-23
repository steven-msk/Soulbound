using SoulboundEngine.Client.ItemSystem.Render;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem {
	public abstract partial class Item {
		public const int DEFAULT_FULL_STACK = 256;

		public abstract string name { get; }
		public virtual int fullStackSize { get; } = DEFAULT_FULL_STACK;

		public abstract ItemRenderData GetRenderData(ItemStack itemStack);

		public virtual ItemStack CreateStack(int quantity = 1) {
			return new ItemStack(this, Mathf.Clamp(quantity, 0, fullStackSize));
		}

		public bool IsStackable() => fullStackSize > 1;

	}
}
