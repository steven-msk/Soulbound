using SoulboundEngine.Item;

#nullable enable

namespace SoulboundEngine.World.Services {
	public interface IInventoryExecutionService {
		void SetStack(int slotIndex, ItemStack stack);
	}
}
