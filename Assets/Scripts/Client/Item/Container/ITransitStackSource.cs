using SoulboundBackend.Client.ItemSystem;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem.Container {
	public interface ITransitStackSource {
		ItemStack? GetTransitStack();
		bool HasTransitStack();
		void SetTransitStack(ItemStack? itemStack);
	}
}
