using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class Items {
    public static readonly BlockItem grassBlock = new BlockItem("Grass Block", Asset<Sprite>("grass_icon"), WorldPrefab("grass_top"), Item.universalMaxStack, () => Blocks.grass, ItemTooltips.DefaultTitle());
    public static readonly BlockItem stoneBlock = new BlockItem("Stone Block", Asset<Sprite>("stone_icon"), null, Item.universalMaxStack, () => Blocks.stone, ItemTooltips.NoTooltip());
    public static readonly BlockItem dirtBlock = new BlockItem("Dirt Block", Asset<Sprite>("dirt_icon"), null, Item.universalMaxStack, () => Blocks.dirt, ItemTooltips.NoTooltip());
    public static readonly BlockItem woodBlock = new BlockItem("Wood Block", Asset<Sprite>("wood_icon"), null, Item.universalMaxStack, () => Blocks.wood, ItemTooltips.NoTooltip());
    public static readonly BlockItem leavesBlock = new BlockItem("Leaves Block", Asset<Sprite>("leaves_icon"), null, Item.universalMaxStack, () => Blocks.leaves, ItemTooltips.NoTooltip());

    private static T Asset<T>(string name) where T : UnityEngine.Object {
        return AssetRegistry.Get<T>(name);
    }

    private static Func<GameObject> ScaledWorldPrefab(int scale, string spriteName) {
        return () => {
            GameObject worldPrefab = Item.InstantiateDefaultWorldPrefab();
            worldPrefab.GetComponent<SpriteRenderer>().sprite = Asset<Sprite>(spriteName);
            worldPrefab.GetComponent<Transform>().localScale = new Vector3(scale, scale, scale);
            return worldPrefab;
        };
    }

    private static Func<GameObject> WorldPrefab(string spriteName) => ScaledWorldPrefab(1, spriteName);
}
