using System;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class GenericItem : ConstructableItemDefinition {
		public override string name { get; }
		public override ItemAspect aspect { get; }
		public override int maxStackSize { get; }

		public GenericItem(
				string name, 
				ItemAspect aspect, 
				int maxStackSize
			)
			: base(name, aspect, maxStackSize) {
			this.name = name;
			this.aspect = aspect;
			this.maxStackSize = maxStackSize;
		}
	}
}
