using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class BlockBreakerItem : Item, IBlockBreakerItem {
		private static readonly Identifier identifier = new("block_breaker_item");
		public override string name => $"Block Breaker Item";

		public BlockBreakerItem() : base(identifier) {
		}

		public int GetBreakLevel(ItemStack itemStack) => 1;

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("bluething", itemStack);
		}
	}
}
