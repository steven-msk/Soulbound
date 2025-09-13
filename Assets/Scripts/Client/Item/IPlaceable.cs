using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IPlaceable : IItemCapability {
		BlockState Place(ItemStack itemStack, BlockPos position);
	}
}
