using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem {
	public partial class Items {
		// TODO: fix AIR item visual case
		public static readonly Item AIR = Register(Blocks.air, Item.Settings.Air());

		public static readonly Item placeableItem = Register(Blocks.movingTickingBlock, 
			Item.Settings.Of("Placeable Item", Item.Settings.RenderFunction("bluething"))
		);
		public static readonly TeleportPlayerItem teleportPlayerItem = Register("teleport_player_item", new TeleportPlayerItem(
			Item.Settings.Of("Move Player Item", Item.Settings.RenderFunction("bluething")).NonStackable()
		));
		public static readonly SpawnEntityItem spawnEntityItem = Register("spawn_entity_item", new SpawnEntityItem(
			Item.Settings.Of("Spawn Entity Item", Item.Settings.RenderFunction("bluething"))
		));
		public static readonly ChargeableItem chargeableItem = Register("chargeable_item", new ChargeableItem(
			Item.Settings.Of("Chargeable Item", Item.Settings.RenderFunction("bluething")).NonStackable()
		));
		public static readonly DebugPointerItem debugPointer = Register("debug_pointer", new DebugPointerItem(
			Item.Settings.Of("Debug Pointer", Item.Settings.RenderFunction("debugPointer")).NonStackable()
		));
		public static readonly InventoryListenerItem inventoryListenerItem = Register("inventory_listener_item", new InventoryListenerItem(
			Item.Settings.Of("Inventory Listener Item", Item.Settings.RenderFunction("bluething"))
		));
		public static readonly BlockBreakerItem blockBreakerItem = Register("block_breaker_item", new BlockBreakerItem(
			Item.Settings.Of("Block Breaker Item", Item.Settings.RenderFunction("bluething")).NonStackable()
		));

		public static TItem Register<TItem>(string id, TItem item) where TItem : Item {
			return Registry<Item>.Register<TItem>(Registries.ITEMS, KeyOf(id), item);
		}

		public static TItem Register<TItem>(Identifier id, TItem item) where TItem : Item {
			return Registry<Item>.Register(Registries.ITEMS, KeyOf(id.ToString()), item);
		}

		public static Item Register(Block block, Item.Settings settings) {
			Item item = Register(Blocks.GetIdentifier(block), new BlockItem(block, settings));
			item.AppendToBlock(block);
			return item;
		}

		public static Identifier GetIdentifier(Item item) {
			return Registries.ITEMS.GetIdentifier(item);
		}

		private static RegistryKey<Item> KeyOf(string id) {
			return RegistryKey<Item>.Of(Registries.ITEMS.GetKey(), Identifier.Of(id));
		}

		public static void Init() { }
	}
}
