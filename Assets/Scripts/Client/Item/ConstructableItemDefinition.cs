using SoulboundBackend.Client.UI.Tooltip;
using System;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract class ConstructableItemDefinition : Item {
		public override string name { get; }
		public override ItemAspect aspect { get; }
		public override int maxStackSize { get; } = Item.DEFAULT_MAX_STACK;
		protected override Func<Item, TooltipData?> tooltipSupplier { get; }
		protected override TooltipRenderer.NodeStyleProvider? nodeStyleProvider { get; }

		public ConstructableItemDefinition(string name, ItemAspect aspect, int maxStackSize,
				Func<Item, TooltipData?> tooltipSupplier, TooltipRenderer.NodeStyleProvider? nodeStyleProvider = null) {
			this.name = name;
			this.aspect = aspect;
			this.maxStackSize = maxStackSize;
			this.tooltipSupplier = tooltipSupplier;
			this.nodeStyleProvider = nodeStyleProvider;
		}

		public ConstructableItemDefinition(string name, ItemAspect aspect, int maxStackSize)
			: this(name, aspect, maxStackSize, (item) => null) {
		}
	}
}
