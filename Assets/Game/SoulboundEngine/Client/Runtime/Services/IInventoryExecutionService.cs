using SoulboundEngine.Client.Item;

#nullable enable

namespace SoulboundEngine.Client.Runtime.Services {
	public interface IInventoryExecutionService {
		void SetStack(int slotIndex, ItemStack stack);
	}
}
