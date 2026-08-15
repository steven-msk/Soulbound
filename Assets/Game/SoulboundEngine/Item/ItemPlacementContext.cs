using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World;

namespace SoulboundEngine.Item {
	public class ItemPlacementContext : ItemUsageContext {
		public bool shouldReplaceExisting { get; } = true;

		public ItemPlacementContext(PlayerEntity player, ItemStack stack, BlockPos blockPos)
			: base(player, blockPos) {
			this.stack = stack;
		}

		public ItemPlacementContext(ItemUsageContext usageContext) 
			: base(usageContext.level, usageContext.player, usageContext.stack, usageContext.blockPos) {
		}
	}
}
