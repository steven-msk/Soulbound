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

    public static readonly BlockItem grassBlock = InjectID(new BlockItem("Grass Block", ItemAspect.Simple("grass_icon", ppu: 16), 210, () => Blocks.grass));
    public static readonly BlockItem stoneBlock = InjectID(new BlockItem("Stone Block", ItemAspect.Simple("stone_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.stone));
    public static readonly BlockItem dirtBlock = InjectID(new BlockItem("Dirt Block", ItemAspect.Simple("dirt_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.dirt));
    public static readonly BlockItem woodBlock = InjectID(new BlockItem("Wood Block", ItemAspect.Simple("wood_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.wood));
    public static readonly BlockItem leavesBlock = InjectID(new BlockItem("Leaves Block", ItemAspect.Simple("leaves_icon", ppu: 8), Item.DEFAULT_MAX_STACK, () => Blocks.leaves));

    public static readonly ArmorItem_test armorItem_test = InjectID(new ArmorItem_test());

#nullable enable

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

#nullable disable

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
