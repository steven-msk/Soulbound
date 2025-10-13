using SoulboundBackend.Client.UI.Tooltip;
using System;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class GenericItem : ConstructableItemDefinition {
		public override string name { get; }
		public override ItemAspect aspect { get; }
		public override int maxStackSize { get; }
		protected override Func<Item, TooltipData?> tooltipSupplier { get; }
		protected override TooltipRenderer.NodeStyleProvider? nodeStyleProvider { get; }

		public GenericItem(
				string name, 
				ItemAspect aspect, 
				int maxStackSize,
				Func<Item, TooltipData?> tooltipSupplier, 
				TooltipRenderer.NodeStyleProvider? nodeStyleProvider = null
			)
			: base(name, aspect, maxStackSize, tooltipSupplier, nodeStyleProvider) {
			this.name = name;
			this.aspect = aspect;
			this.maxStackSize = maxStackSize;
			this.tooltipSupplier = tooltipSupplier;
		}
	}
}
