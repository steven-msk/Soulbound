using SoulboundEngine.Client.World.EntitySystem;

namespace SoulboundEngine.Client.ItemSystem {
	public interface IItemCollector {
		Entity GetEntity();
		bool TryPickupStack(ItemStack itemStack);
	}
}
