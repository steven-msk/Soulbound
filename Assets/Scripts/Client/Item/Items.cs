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
	public partial class Items : ICachedRegistry<Item> {
		[ItemCache(nameof(grassBlock))] public static BlockItem grassBlock => Lookup(() => new BlockItem("Grass Block", ItemAspect.Simple("grass_icon", ppu: 16), 210, () => Blocks.grass));
		[ItemCache(nameof(stoneBlock))] public static BlockItem stoneBlock => Lookup(() => new BlockItem("Stone Block", ItemAspect.Simple("stone_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.stone));
		[ItemCache(nameof(dirtBlock))] public static BlockItem dirtBlock => Lookup(() => new BlockItem("Dirt Block", ItemAspect.Simple("dirt_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.dirt));
		[ItemCache(nameof(woodBlock))] public static BlockItem woodBlock => Lookup(() => new BlockItem("Wood Block", ItemAspect.Simple("wood_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.wood));
		[ItemCache(nameof(leavesBlock))] public static BlockItem leavesBlock => Lookup(() => new BlockItem("Leaves Block", ItemAspect.Simple("leaves_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.leaves));

		[ItemCache(nameof(armorItem_test))] public static ArmorItem_test armorItem_test => Lookup(() => new ArmorItem_test());
		[ItemCache(nameof(consumableStatItem_test))] public static ConsumableStatItem_test consumableStatItem_test => Lookup(() => new ConsumableStatItem_test());
		[ItemCache(nameof(statItem_test))] public static StatItem_test statItem_test => Lookup(() => new StatItem_test());

		[ItemCache(nameof(toolItem_test))] public static ToolItem_test toolItem_test => Lookup(() => new ToolItem_test());

		static Items() {
            foreach (var property in typeof(Items).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
                var cacheAttribute = property.GetCustomAttribute<ItemCache>();
                if (cacheAttribute != null) {
                    ICachedRegistry<Item>.RegisterCachedReference<ItemCache>(cacheAttribute, property);
                }
            }
        }

		private static TItem Lookup<TItem>(Func<TItem> instanceSupplier, [CallerMemberName] string propertyName = null) where TItem : Item {
			return (TItem)ICachedRegistry<Item>.Lookup(propertyName, instanceSupplier);
		}

		public static Item ByHashedID(int hashedID) {
			if (ICachedRegistry<Item>.cached.TryGetValue(hashedID, out Item item)) {
				return item;
			}
			if (ICachedRegistry<Item>.cachedReferences.TryGetValue(hashedID, out Func<Item> reference)) {
				return reference.Invoke();
			}
			throw new KeyNotFoundException($"Item hashedID {hashedID} not found.");
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ItemCache : Attribute, ICachedReferenceAttribute {
		public string propertyName { get; set; }

		public ItemCache(string propertyName) {
			this.propertyName = propertyName;
		}
	}

	[JsonConverter(typeof(Item.ItemJsonConverter))]
	public abstract partial class Item : IHashableReference {
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
