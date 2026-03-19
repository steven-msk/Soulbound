using SoulboundBackend.Client.ItemSystem.Container;
using SoulboundBackend.Client.ItemSystem.Container.View;
using SoulboundBackend.Client.Players;

namespace SoulboundBackend.Client.UI.Screens {
	public sealed class WorldScreen : Screen {
		private readonly Player player;

		public WorldScreen(Player player) {
			this.player = player;
		}

		public override IScreenObject BuildObject(IScreenObjectFactory objFactory) {
			return base.BuildObject(new WorldSessionScreenFactory(objFactory));
		}

		protected override void OnBuild(IScreenObject screenObject) {
			player.SetTransitStackSource((ITransitStackSource)screenObject);

			InventoryUIBuilder inventoryUIBuilder = new(player.GetInventory(), player.GetHotbar());
			inventoryUIBuilder.Build(
				(IItemContainerScreenScope)screenObject,
				out IItemContainerHandle inventory,
				out IItemContainerHandle hotbar
			);
			inventoryUIBuilder.FixInventoryPosition(inventory, hotbar);
		}
	}
}
