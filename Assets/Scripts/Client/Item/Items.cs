using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Plastic.Newtonsoft.Json;

namespace SoulboundBackend.Client.ItemSystem {
	public partial class Items {
		private static ConcurrentDictionary<int, Item> cached = new();
		private static ConcurrentDictionary<int, Func<Item>> cachedReferences = new();

		[ItemCache(nameof(grassBlock))] public static BlockItem grassBlock => Lookup(() => new BlockItem("Grass Block", ItemAspect.Simple("grass_icon", ppu: 16), 210, () => Blocks.grass));
		[ItemCache(nameof(stoneBlock))] public static BlockItem stoneBlock => Lookup("stoneBlock", () => new BlockItem("Stone Block", ItemAspect.Simple("stone_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.stone));
		[ItemCache(nameof(dirtBlock))] public static BlockItem dirtBlock => Lookup("dirtBlock", () => new BlockItem("Dirt Block", ItemAspect.Simple("dirt_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.dirt));
		[ItemCache(nameof(woodBlock))] public static BlockItem woodBlock => Lookup("woodBlock", () => new BlockItem("Wood Block", ItemAspect.Simple("wood_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.wood));
		[ItemCache(nameof(leavesBlock))] public static BlockItem leavesBlock => Lookup("leavesBlock", () => new BlockItem("Leaves Block", ItemAspect.Simple("leaves_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.leaves));

		[ItemCache(nameof(armorItem_test))] public static ArmorItem_test armorItem_test => Lookup("armorItem_test", () => new ArmorItem_test());
		[ItemCache(nameof(consumableStatItem_test))] public static ConsumableStatItem_test consumableStatItem_test => Lookup("consumableStatItem_test", () => new ConsumableStatItem_test());
		[ItemCache(nameof(statItem_test))] public static StatItem_test statItem_test => Lookup("statItem_test", () => new StatItem_test());

		static Items() {
            foreach (var property in typeof(Items).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
                var cacheAttribute = property.GetCustomAttribute<ItemCache>();
                if (cacheAttribute != null) {
                    RegisterItemCache(cacheAttribute, property);
                }
            }
        }

		private static TItem Lookup<TItem>(string key, Func<TItem> instanceSupplier) where TItem : Item {
			int hash = HashHelper.StableHash(key);

            return (TItem)cached.GetOrAdd(hash, hashedID => {
                TItem item = instanceSupplier.Invoke();
                item.hashedID = hashedID;
                return item;
            });
        }

		private static TItem Lookup<TItem>(Func<TItem> instanceSupplier, [CallerMemberName] string propertyName = null) where TItem : Item {
			return Lookup<TItem>(propertyName, instanceSupplier);
		}

		public static Item ByHashedID(int hashedID) {
			if (cached.TryGetValue(hashedID, out Item item)) {
				return item;
			}
			if (cachedReferences.TryGetValue(hashedID, out Func<Item> reference)) {
				return reference.Invoke();
			}
			throw new KeyNotFoundException($"Item hashedID {hashedID} not found.");
		}

		private static void RegisterItemCache(ItemCache itemCache, PropertyInfo property) {
			var getter = property.GetGetMethod();
			if (getter == null) {
                throw new NotSupportedException("No getter found for item property: " + property);
            }

			Func<Item> acessor = () => (Item)getter.Invoke(null, null);
			int hash = HashHelper.StableHash(itemCache.PropertyName);
			cachedReferences[hash] = acessor;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ItemCache : Attribute {
		public string PropertyName { get; set; }

		public ItemCache(string propertyName) {
			this.PropertyName = propertyName;
		}
	}

	[JsonConverter(typeof(Item.ItemJsonConverter))]
	public abstract partial class Item {
		public int hashedID { get; set; }

		public sealed class ItemJsonConverter : JsonConverter<Item> {
			public override Item ReadJson(JsonReader reader, Type objectType, Item existingValue, bool hasExistingValue, JsonSerializer serializer) {
				int id = Convert.ToInt32(reader.Value);
				return Items.ByHashedID(id);
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
