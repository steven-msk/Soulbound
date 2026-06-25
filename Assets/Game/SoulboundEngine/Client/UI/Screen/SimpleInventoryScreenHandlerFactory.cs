using SoulboundEngine.Client.Player;
using System;

namespace SoulboundEngine.Client.UI.Screen {
	public class SimpleInventoryScreenHandlerFactory : IInventoryScreenHandlerFactory {
		private readonly Func<PlayerInventory, PlayerEntity, InventoryScreenHandler> factory;

		public SimpleInventoryScreenHandlerFactory(Func<PlayerInventory, PlayerEntity, InventoryScreenHandler> factory) {
			this.factory = factory;
		}

		public InventoryScreenHandler Create(PlayerInventory playerInventory, PlayerEntity player) {
			return this.factory(playerInventory, player);
		}
	}
}
