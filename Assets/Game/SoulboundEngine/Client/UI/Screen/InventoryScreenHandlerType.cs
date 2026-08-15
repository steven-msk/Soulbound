using SoulboundEngine.World.Player;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.UI.Screen {
	public abstract class InventoryScreenHandlerType {
		public delegate THandler Factory<THandler>(PlayerInventory playerInventory) where THandler : InventoryScreenHandler;

		public static InventoryScreenHandlerType<PlayerInventoryScreenHandler> PLAYER_INVENTORY = Register("player_inventory", 
			playerInventory => new PlayerInventoryScreenHandler(PLAYER_INVENTORY, playerInventory)
		);
		public static InventoryScreenHandlerType<ChestInventoryScreenHandler> CHEST = Register("chest",
			playerInventory => new ChestInventoryScreenHandler(CHEST, playerInventory)
		);

		private static InventoryScreenHandlerType<THandler> Register<THandler>(string id, Factory<THandler> factory) where THandler : InventoryScreenHandler {
			return Registry<InventoryScreenHandlerType>.Register(
				registry: Registries.INVENTORY_SCREEN_HANDLES,
				key: RegistryKey<InventoryScreenHandlerType>.Of(Registries.INVENTORY_SCREEN_HANDLES.GetKey(), Identifier.Of(id)),
				value: new InventoryScreenHandlerType<THandler>(factory)
			);
		}

		public static void Init() { }
	}

	public class InventoryScreenHandlerType<THandler> : InventoryScreenHandlerType where THandler : InventoryScreenHandler {
		private readonly Factory<THandler> factory;

		public InventoryScreenHandlerType(Factory<THandler> factory) {
			this.factory = factory;
		}

		public THandler Create(PlayerInventory playerInventory) {
			return this.factory(playerInventory);
		}
	}
}
