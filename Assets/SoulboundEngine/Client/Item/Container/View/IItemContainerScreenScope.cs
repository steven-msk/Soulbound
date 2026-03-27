using SoulboundEngine.Client.UI.Screens;
namespace SoulboundEngine.Client.ItemSystem.Container.View {
	public interface IItemContainerScreenScope : IScreenObject, IItemContainerScope {
		void AddItemContainer(UIItemContainerNode node);
		void RemoveItemContainer(UIItemContainerNode node);

		void SetTransitStackHandle(ITransitStackHandle transitStackHandle);
		void RemoveTransitStack();
	}
}
