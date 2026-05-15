using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldScreen : UxmlScreen {
		private readonly Player player;
		private readonly ItemRenderManager itemRenderManager;
		private PlayerInventoryHandle inventoryHandle;

		public WorldScreen(ItemRenderManager itemRenderManager, Player player) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("WorldScreen"))) {
			this.itemRenderManager = itemRenderManager;
			this.player = player;
		}

		public override bool ReturnWithEscape => false;

		protected override void OnBind(VisualElement root) {
			this.inventoryHandle = new PlayerInventoryHandle(this.player.GetInventory(), this.itemRenderManager);
			this.inventoryHandle.OnBind(root.Q<VisualElement>("Inventory"));
		}

		public override void OnDispose(IScreenHandle handle) {
			this.inventoryHandle.Dispose();
		}

		//public override IScreenObject BuildObject(IScreenObjectFactory objFactory) {
		//	return base.BuildObject(new WorldSessionScreenFactory(this.itemRenderManager, objFactory));
		//}

		//protected override void OnBuild(IScreenHandle handle) {
		//	this.player.SetTransitStackSource((ITransitStackSource)handle);

		//	PlayerInventoryRenderer inventoryUIBuilder = new(this.itemRenderManager, this.player.GetInventory());
		//	inventoryUIBuilder.Build(
		//		(IItemContainerScreenScope)handle,
		//		out IItemContainerHandle inventory,
		//		out IItemContainerHandle hotbar
		//	);
		//	inventoryUIBuilder.FixInventoryPosition(inventory, hotbar);
		//}
	}
}
