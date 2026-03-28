using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class BlockBreakerItem : Item, IBlockBreakerItem {
		public override string name => $"Block Breaker Item";

		public BlockBreakerItem() : base("blockBreakerItem") {
		}

		public int GetBreakLevel(ItemStack itemStack) => 1;

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData(new AssetKey("bluething"), itemStack);
		}
	}
}
