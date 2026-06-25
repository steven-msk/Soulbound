using SoulboundEngine.Client.World.Entity;

namespace SoulboundEngine.Client.Item {
	public interface IItemCollector {
		Entity GetEntity();
		bool TryPickupStack(ItemStack itemStack);
	}
}
