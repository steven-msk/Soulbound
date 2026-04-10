using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class BlockBreakerItem : Item, IBlockBreakerItem {
		public override string name => $"Block Breaker Item";

		public int GetBreakLevel(ItemStack itemStack) => 1;

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("bluething", itemStack);
		}
	}
}
