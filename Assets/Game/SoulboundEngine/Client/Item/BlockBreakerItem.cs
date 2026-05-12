using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class BlockBreakerItem : Item, IBlockBreakerItem {
		public BlockBreakerItem(Settings settings) : base(settings) {
		}

		public int GetBreakLevel(ItemStack itemStack) => 1;
	}
}
