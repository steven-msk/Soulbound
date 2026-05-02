using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.ItemSystem.Container.View;
using SoulboundEngine.Client.Players;

namespace SoulboundEngine.Client.UI.Screens {
	public sealed class WorldScreen : Screen {
		private readonly Player player;

		public WorldScreen(Player player) {
			this.player = player;
			this.supportsEscapePop = false;
		}

		public override IScreenObject BuildObject(IScreenObjectFactory objFactory) {
			return base.BuildObject(new WorldSessionScreenFactory(objFactory));
		}

		protected override void OnBuild(IScreenObject screenObject) {
			this.player.SetTransitStackSource((ITransitStackSource)screenObject);

			InventoryUIBuilder inventoryUIBuilder = new(this.player.GetInventory());
			inventoryUIBuilder.Build(
				(IItemContainerScreenScope)screenObject,
				out IItemContainerHandle inventory,
				out IItemContainerHandle hotbar
			);
			inventoryUIBuilder.FixInventoryPosition(inventory, hotbar);
		}
	}
}
