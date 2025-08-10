using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public static partial class Blocks {
    // REMINDER: since block state properties are unavailable as of right now, keep in mind that block behavior definitions might change when they have actual purpose

    public static readonly Block air = new GenericBlock("Air", Tile("air"), null);
    public static readonly Block grass = new GenericBlock("Grass Block", Tile("grass"), Items.grassBlock);
    public static readonly Block dirt = new GenericBlock("Dirt Block", Tile("dirt"), Items.dirtBlock);
    public static readonly Block stone = new GenericBlock("Stone Block", Tile("stone"), Items.stoneBlock);
    public static readonly Block wood = new GenericBlock("Wood", Tile("wood"), Items.woodBlock);
    public static readonly Block leaves = new GenericBlock("Leaves", Tile("leaves"), Items.leavesBlock, _ => BlockBehaviors.DropIfPlayerBroke());

    private static TAsset Resource<TAsset, TGroup>(string name)
            where TAsset : UnityEngine.Object
            where TGroup : ResourceGroups.IResourceGroupDefinition<TAsset> {
        return ResourceManager.Get<TAsset, TGroup>(name);
    }

    private static TileBase Tile(string name) {
        return Resource<TileBase, ResourceGroups.Tiles>(name);
    }
}
