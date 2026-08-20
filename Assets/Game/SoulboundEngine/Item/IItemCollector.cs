using SoulboundEngine.World.Entity;

namespace SoulboundEngine.Item {
	public interface IItemCollector {
		Entity GetEntity();
		bool TryPickupStack(ItemStack itemStack);
	}
}
