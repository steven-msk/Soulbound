namespace SoulboundEngine.Client.World {
	using SoulboundEngine.Client.UI.Screen;
	using SoulboundEngine.Client.Util;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Inventory;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;

#nullable enable

	public sealed class ClientPlayerEntity : PlayerEntity {
		private readonly SoulboundClient client;
		private IScreenHandle? activeInventoryScreen;
		private InventoryScreenHandler? activeInventoryScreenHandler;
		private bool isInventoryOpen;
		private IScreenHandle? signEditScreen;

		public ClientPlayerEntity(SoulboundClient client, Level level) 
			: base(level) {
			this.client = client;
		}

		public override void OpenInventoryScreen(IInventoryScreenHandlerFactory handlerFactory) {
			if (this.activeInventoryScreen != null) return;

			InventoryScreenHandler handler = handlerFactory.Create(this.GetInventory(), this);
			this.activeInventoryScreenHandler = handler;

			this.activeInventoryScreen = InventoryScreens.Open(handler, this.client, this.GetInventory(), this);
			this.isInventoryOpen = true;
		}

		public override void CloseInventoryScreen() {
			if (this.activeInventoryScreen == null) return;

			this.client.CloseScreen(this.activeInventoryScreen);
			this.activeInventoryScreenHandler!.OnClosed(this);

			this.activeInventoryScreen = null;
			this.activeInventoryScreenHandler = null;
			this.isInventoryOpen = false;
		}

		public override bool IsInventoryOpen() => this.isInventoryOpen;

		public override InventoryScreenHandler? GetInventoryScreenHandler() => this.activeInventoryScreenHandler;

		public override bool OpenSignEditScreen(SignTileEntity signEntity) {
			if (this.signEditScreen != null) return false;
			this.signEditScreen = this.client.OpenScreen(new SignEditScreen(signEntity, this));
			return true;
		}

		public void CloseSignEditScreen() {
			if (this.signEditScreen == null) return;
			this.client.CloseScreen(this.signEditScreen);
			this.signEditScreen = null;
		}

		public override Vec2d GetWorldPointerPos() {
			return this.client.ScreenToWorldPoint(this.GetScreenPointerPos().ToVector2());
		}
	}
}
