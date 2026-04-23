using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.ItemSystem {
	public partial class Items {
		public static readonly StackItem stackitem_1 = Register("stack_item_1", new StackItem(1));
		public static readonly StackItem stackitem_10 = Register("stack_item_10", new StackItem(10));
		public static readonly StackItem stackitem_256 = Register("stack_item_256", new StackItem(Item.DEFAULT_FULL_STACK));
		public static readonly StackItem stackitem_64 = Register("stack_item_64", new StackItem(64));
		public static readonly PlaceableItem placeableItem = Register("placeable_item", new PlaceableItem());
		public static readonly TeleportPlayerItem teleportPlayerItem = Register("teleport_player_item", new TeleportPlayerItem());
		public static readonly SpawnEntityItem spawnEntityItem = Register("spawn_entity_item", new SpawnEntityItem());
		public static readonly ChargeableItem chargeableItem = Register("chargeable_item", new ChargeableItem());
		public static readonly DebugPointerItem debugPointer = Register("debug_pointer", new DebugPointerItem());
		public static readonly InventoryListenerItem inventoryListenerItem = Register("inventory_listener_item", new InventoryListenerItem());
		public static readonly BlockBreakerItem blockBreakerItem = Register("block_breaker_item", new BlockBreakerItem());

		private static TItem Register<TItem>(string id, TItem item) where TItem : Item {
			return Registries.Register(Registries.ITEMS, Identifier.Of(id), item);
		}

		public static Identifier GetIdentifier(Item item) {
			return Registries.ITEMS.GetIdentifier(item);
		}

		public static void Init() { }
	}
}
