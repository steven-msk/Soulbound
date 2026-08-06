using System;

namespace SoulboundEngine.Client.Item {
	public sealed class StackReference {
		public static readonly StackReference EMPTY = new(stack => { }, () => ItemStack.EMPTY);
		private readonly Action<ItemStack> setter;
		private readonly Func<ItemStack> getter;

		public StackReference(Action<ItemStack> setter, Func<ItemStack> getter) {
			this.setter = setter;
			this.getter = getter;
		}

		public ItemStack Get() => this.getter();

		public void Set(ItemStack stack) => this.setter(stack);
	}
}
