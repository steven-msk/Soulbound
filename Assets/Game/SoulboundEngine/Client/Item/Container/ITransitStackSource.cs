using SoulboundEngine.Client.ItemSystem;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public interface ITransitStackSource {
		ItemStack? GetTransitStack();
		bool HasTransitStack();
		void SetTransitStack(ItemStack? itemStack);
	}
}
