using SoulboundBackend.Client.UI.Tooltip;
using System;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public static class ItemTooltips {
		public static Func<Item, Tooltip?> NoTooltip() => item => null;

		//public static Func<Item, AbstractTooltip?> DefaultTitle() => item => CompoundTooltip.Of(Tooltip.Title(item.name));
	}
}
