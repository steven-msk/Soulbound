using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public partial class Items : IResourceModule {
    public static readonly BlockItem grassBlock = new BlockItem("Grass Block", Icon("grass_icon"), WorldPrefab("grass_top"), Item.universalMaxStack, () => Blocks.grass);
    public static readonly BlockItem stoneBlock = new BlockItem("Stone Block", Icon("stone_icon"), null, Item.universalMaxStack, () => Blocks.stone);
    public static readonly BlockItem dirtBlock = new BlockItem("Dirt Block", Icon("dirt_icon"), null, Item.universalMaxStack, () => Blocks.dirt);
    public static readonly BlockItem woodBlock = new BlockItem("Wood Block", Icon("wood_icon"), null, Item.universalMaxStack, () => Blocks.wood);
    public static readonly BlockItem leavesBlock = new BlockItem("Leaves Block", Icon("leaves_icon"), null, Item.universalMaxStack, () => Blocks.leaves);

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
}
