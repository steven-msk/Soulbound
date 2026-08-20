using SoulboundEngine.Common.Math;
using SoulboundEngine.Item;

namespace SoulboundEngine.World.Services {
	public interface IPlayerExecutionService {
		IInventoryExecutionService Inventory { get; }
		void SetPos(Vec2d pos);
		bool TryAddItemStack(ItemStack itemStack);
	}
}
