using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SoulboundBackend.Common.Patterns;

namespace SoulboundBackend.Client.ItemSystem {
	public partial class Items : ICachedRegistry<Item> {

		public static readonly StackItem stackitem_1 = new(1);
		public static readonly StackItem stackitem_10 = new(10);
		public static readonly StackItem stackitem_256 = new(Item.DEFAULT_FULL_STACK);
		public static readonly StackItem stackitem_64 = new(64);
		public static readonly PlaceableItem placeableItem = new();
		public static readonly TeleportPlayerItem teleportPlayerItem = new();
		public static readonly SpawnEntityItem spawnEntityItem = new();
		public static readonly ChargeableItem chargeableItem = new();
		public static readonly DebugPointerItem debugPointer = new();
		public static readonly InventoryListenerItem inventoryListenerItem = new();
		public static readonly BlockBreakerItem blockBreakerItem = new();

		[Obsolete]
		static Items() {
            foreach (var property in typeof(Items).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
                var cacheAttribute = property.GetCustomAttribute<ItemCache>();
                if (cacheAttribute != null) {
                    ICachedRegistry<Item>.RegisterCachedReference<ItemCache>(cacheAttribute, property);
                }
            }
        }

		[Obsolete]
		private static TItem Lookup<TItem>(Func<TItem> instanceSupplier, [CallerMemberName] string propertyName = null) where TItem : Item {
			return (TItem)ICachedRegistry<Item>.Lookup(propertyName, instanceSupplier);
		}

		[Obsolete]
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
