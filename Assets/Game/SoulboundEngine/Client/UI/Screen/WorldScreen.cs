using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.ItemSystem.Container.View;
using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.Render.Item;

namespace SoulboundEngine.Client.UI.Screens {
	public sealed class WorldScreen : Screen {
		private readonly Player player;
		private readonly ItemRenderManager itemRenderManager;

		public WorldScreen(ItemRenderManager itemRenderManager, Player player) {
			this.itemRenderManager = itemRenderManager;
			this.player = player;
			this.supportsEscapePop = false;
		}

		public override IScreenObject BuildObject(IScreenObjectFactory objFactory) {
			return base.BuildObject(new WorldSessionScreenFactory(objFactory));
		}

		protected override void OnBuild(IScreenObject screenObject) {
			this.player.SetTransitStackSource((ITransitStackSource)screenObject);

			PlayerInventoryRenderer inventoryUIBuilder = new(this.itemRenderManager, this.player.GetInventory());
			inventoryUIBuilder.Build(
				(IItemContainerScreenScope)screenObject,
				out IItemContainerHandle inventory,
				out IItemContainerHandle hotbar
			);
			inventoryUIBuilder.FixInventoryPosition(inventory, hotbar);
		}
	}
}
