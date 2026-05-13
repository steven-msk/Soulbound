using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.ItemSystem.Container.View;
using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.Render.Item;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldScreen : Screen {
		private readonly Player player;
		private readonly ItemRenderManager itemRenderManager;

		public WorldScreen(ItemRenderManager itemRenderManager, Player player) {
			this.itemRenderManager = itemRenderManager;
			this.player = player;
		}

		public override bool ReturnWithEscape => false;

		//public override IScreenObject BuildObject(IScreenObjectFactory objFactory) {
		//	return base.BuildObject(new WorldSessionScreenFactory(this.itemRenderManager, objFactory));
		//}

		protected override void OnBuild(IScreenHandle handle) {
			this.player.SetTransitStackSource((ITransitStackSource)handle);

			PlayerInventoryRenderer inventoryUIBuilder = new(this.itemRenderManager, this.player.GetInventory());
			inventoryUIBuilder.Build(
				(IItemContainerScreenScope)handle,
				out IItemContainerHandle inventory,
				out IItemContainerHandle hotbar
			);
			inventoryUIBuilder.FixInventoryPosition(inventory, hotbar);
		}
	}
}
