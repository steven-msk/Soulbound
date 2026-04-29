using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem {
	public partial class Items {
		private static readonly Dictionary<Block, Item> blockItems = new();
		public static readonly Item AIR = Register(Blocks.air, Item.Settings.Air());

		public static readonly Item placeableItem = Register(Blocks.movingTickingBlock, 
			Item.Settings.Stackable("Placeable Item", Item.DEFAULT_FULL_STACK, Item.Settings.RenderFunction("bluething"))
		);
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

		public static TItem Register<TItem>(string id, TItem item) where TItem : Item {
			return Registry<Item>.Register(Registries.ITEMS, Identifier.Of(id), item);
		}

		public static TItem Register<TItem>(Identifier id, TItem item) where TItem : Item {
			return Registry<Item>.Register(Registries.ITEMS, id, item);
		}

		public static Item Register(Block block, Item.Settings settings) {
			Item item = Register(Blocks.GetIdentifier(block), new BlockItem(block, settings));
			blockItems.Add(block, item);
			return item;
		}

		public static Identifier GetIdentifier(Item item) {
			return Registries.ITEMS.GetIdentifier(item);
		}

		public static Item FromBlock(Block? block) {
			block ??= Blocks.air;
			return blockItems.GetValueOrDefault(block, AIR);
		}

		public static void Init() { }
	}
}
