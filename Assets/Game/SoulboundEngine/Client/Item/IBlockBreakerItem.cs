using System;

namespace SoulboundEngine.Client.Item {

	// might be renamed to something more appropiate
	[Obsolete]
	public interface IBlockBreakerItem {
		int GetBreakLevel(ItemStack itemStack);
	}
}
