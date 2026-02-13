using System;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract class ConstructableItemDefinition : Item {
		public override string name { get; }
		public override ItemAspect aspect { get; }
		public override int maxStackSize { get; } = Item.DEFAULT_MAX_STACK;
		public ConstructableItemDefinition(
				string name,
				ItemAspect aspect, 
				int maxStackSize
			) {
			this.name = name;
			this.aspect = aspect;
			this.maxStackSize = maxStackSize;
		}
	}
}
