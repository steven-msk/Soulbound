using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Plastic.Newtonsoft.Json;

namespace SoulboundBackend.Client.ItemSystem {
	public partial class Items : IResourceModule {
		//private static int idCounter = 0;
		private static Dictionary<int, Item> itemsById = new();
		private static ConcurrentDictionary<int, Item> cached = new();

		public static BlockItem grassBlock => Lookup("grassBlock", () => new BlockItem("Grass Block", ItemAspect.Simple("grass_icon", ppu: 16), 210, () => Blocks.grass));
		public static BlockItem stoneBlock => Lookup("stoneBlock", () => new BlockItem("Stone Block", ItemAspect.Simple("stone_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.stone));
		public static BlockItem dirtBlock => Lookup("dirtBlock", () => new BlockItem("Dirt Block", ItemAspect.Simple("dirt_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.dirt));
		public static BlockItem woodBlock => Lookup("woodBlock", () => new BlockItem("Wood Block", ItemAspect.Simple("wood_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.wood));
		public static BlockItem leavesBlock => Lookup("leavesBlock", () => new BlockItem("Leaves Block", ItemAspect.Simple("leaves_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.leaves));

		public static ArmorItem_test armorItem_test => Lookup("armorItem_test", () => new ArmorItem_test());
		public static ConsumableStatItem_test consumableStatItem_test => Lookup("consumableStatItem_test", () => new ConsumableStatItem_test());
		public static StatItem_test statItem_test => Lookup("statItem_test", () => new StatItem_test());

		private static TItem Lookup<TItem>(string key, Func<TItem> instanceSupplier) where TItem : Item {
			int hash = StableHash(key);

            return (TItem)cached.GetOrAdd(hash, hashedID => {
                TItem item = instanceSupplier.Invoke();
                item.hashedID = hashedID;
                return item;
            });
        }

		public static int StableHash(string id) {
			unchecked {
                const int offset = (int)2166136261;
                const int prime = 16777619;
                int hash = offset;
                foreach (char c in id) {
                    hash = (hash ^ c) * prime;
				}
                return hash;
            }
		}

#nullable enable

		//private static TItem InjectID<TItem>(TItem item) where TItem : Item {
		//	item.hashedID = idCounter++;
		//	itemsById[item.hashedID] = item;
		//	return item;
		//}

		public static Item ByID(int id) {
			if (itemsById.TryGetValue(id, out Item item)) {
				return item;
			}
			throw new KeyNotFoundException($"Item ID {id} not found.");
		}
	}

#nullable disable

	[JsonConverter(typeof(Item.ItemJsonConverter))]
	public abstract partial class Item {
		public int hashedID { get; set; }

		public sealed class ItemJsonConverter : JsonConverter<Item> {
			public override Item ReadJson(JsonReader reader, Type objectType, Item existingValue, bool hasExistingValue, JsonSerializer serializer) {
				int id = Convert.ToInt32(reader.Value);
				return Items.ByID(id);
			}

			public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer) {
				writer.WriteValue(value.hashedID);
			}
		}

		public override bool Equals(object obj) {
			return obj is Item other && other.hashedID == this.hashedID;
		}
	}
}
