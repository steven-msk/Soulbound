using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public partial class Items : IResourceModule {
    private static int idCounter = 0;
    private static Dictionary<int, Item> itemsById = new();

    public static readonly BlockItem grassBlock = InjectID(new BlockItem("Grass Block", Icon("grass_icon"), WorldPrefab("grass_top"), Item.universalMaxStack, () => Blocks.grass));
    public static readonly BlockItem stoneBlock = InjectID(new BlockItem("Stone Block", Icon("stone_icon"), null, Item.universalMaxStack, () => Blocks.stone));
    public static readonly BlockItem dirtBlock = InjectID(new BlockItem("Dirt Block", Icon("dirt_icon"), null, Item.universalMaxStack, () => Blocks.dirt));
    public static readonly BlockItem woodBlock = InjectID(new BlockItem("Wood Block", Icon("wood_icon"), null, Item.universalMaxStack, () => Blocks.wood));
    public static readonly BlockItem leavesBlock = InjectID(new BlockItem("Leaves Block", Icon("leaves_icon"), null, Item.universalMaxStack, () => Blocks.leaves));

    private static Sprite Icon(string name) {
        return IResourceModule.Resource<Sprite, ResourceGroups.Items.Icons>(name);
    }

    private static Func<GameObject> ScaledWorldPrefab(int scale, string spriteName) {
        return () => {
            GameObject worldPrefab = Item.InstantiateDefaultWorldPrefab();
            worldPrefab.GetComponent<SpriteRenderer>().sprite = Icon(spriteName);
            worldPrefab.GetComponent<Transform>().localScale = new Vector3(scale, scale, scale);
            return worldPrefab;
        };
    }

    private static Func<GameObject> WorldPrefab(string spriteName) => ScaledWorldPrefab(1, spriteName);

    private static TItem InjectID<TItem>(TItem item) where TItem : Item {
        item.id = idCounter++;
        itemsById[item.id] = item;
        return item;
    }

    public static Item ByID(int id) {
        if (itemsById.TryGetValue(id, out Item item)) {
            return item;
        }
        throw new KeyNotFoundException($"Item ID {id} not found.");
	}
}

[JsonConverter(typeof(Item.ItemJsonConverter))]
abstract partial class Item {
    public int id { get; internal set; }

	public sealed class ItemJsonConverter : JsonConverter<Item> {
		public override Item ReadJson(JsonReader reader, Type objectType, Item existingValue, bool hasExistingValue, JsonSerializer serializer) {
            int id = Convert.ToInt32(reader.Value);
			return Items.ByID(id);
		}

		public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer) {
			writer.WriteValue(value.id);
		}
	}

	public override bool Equals(object obj) {
        return obj is Item other && other.id == this.id;
	}
}
