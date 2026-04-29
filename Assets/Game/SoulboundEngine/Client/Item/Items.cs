using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.ItemSystem {
	public partial class Items {
		public static readonly PlaceableItem placeableItem = Register("placeable_item", new PlaceableItem(
			Item.Settings.Stackable("Placeable Item", Item.DEFAULT_FULL_STACK, Item.Settings.RenderFunction("bluething"))
		));
		public static readonly TeleportPlayerItem teleportPlayerItem = Register("teleport_player_item", new TeleportPlayerItem(
			Item.Settings.NonStackable("Move Player Item", Item.Settings.RenderFunction("bluething"))
		));
		public static readonly SpawnEntityItem spawnEntityItem = Register("spawn_entity_item", new SpawnEntityItem(
			Item.Settings.Stackable("Spawn Entity Item", Item.DEFAULT_FULL_STACK, Item.Settings.RenderFunction("bluething"))
		));
		public static readonly ChargeableItem chargeableItem = Register("chargeable_item", new ChargeableItem(
			Item.Settings.NonStackable("Chargeable Item", Item.Settings.RenderFunction("bluething"))
		));
		public static readonly DebugPointerItem debugPointer = Register("debug_pointer", new DebugPointerItem(
			Item.Settings.NonStackable("Debug Pointer", Item.Settings.RenderFunction("debugPointer"))
		));
		public static readonly InventoryListenerItem inventoryListenerItem = Register("inventory_listener_item", new InventoryListenerItem(
			Item.Settings.Stackable("Inventory Listener Item", Item.DEFAULT_FULL_STACK, Item.Settings.RenderFunction("bluething"))
		));
		public static readonly BlockBreakerItem blockBreakerItem = Register("block_breaker_item", new BlockBreakerItem(
			Item.Settings.NonStackable("Block Breaker Item", Item.Settings.RenderFunction("bluething"))
		));

		private static TItem Register<TItem>(string id, TItem item) where TItem : Item {
			return Registries.Register(Registries.ITEMS, Identifier.Of(id), item);
		}

		public static Identifier GetIdentifier(Item item) {
			return Registries.ITEMS.GetIdentifier(item);
		}

		public static void Init() { }
	}
}
