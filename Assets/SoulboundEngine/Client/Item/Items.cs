using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.ItemSystem {
	public partial class Items {
		public static readonly StackItem stackitem_1 = Register(new StackItem(1));
		public static readonly StackItem stackitem_10 = Register(new StackItem(10));
		public static readonly StackItem stackitem_256 = Register(new StackItem(Item.DEFAULT_FULL_STACK));
		public static readonly StackItem stackitem_64 = Register(new StackItem(64));
		public static readonly PlaceableItem placeableItem = Register(new PlaceableItem());
		public static readonly TeleportPlayerItem teleportPlayerItem = Register(new TeleportPlayerItem());
		public static readonly SpawnEntityItem spawnEntityItem = Register(new SpawnEntityItem());
		public static readonly ChargeableItem chargeableItem = Register(new ChargeableItem());
		public static readonly DebugPointerItem debugPointer = Register(new DebugPointerItem());
		public static readonly InventoryListenerItem inventoryListenerItem = Register(new InventoryListenerItem());
		public static readonly BlockBreakerItem blockBreakerItem = Register(new BlockBreakerItem());

		private static TItem Register<TItem>(TItem item) where TItem : Item {
			return Registries.Register(Registries.ITEMS, item.GetIdentifier(), item);
		}

		public static void Init() { }
	}
}
