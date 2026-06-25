using SoulboundEngine.Client.Item;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public interface ITransitStackSource {
		ItemStack? GetTransitStack();
		bool HasTransitStack();
		void SetTransitStack(ItemStack? itemStack);
	}
}
