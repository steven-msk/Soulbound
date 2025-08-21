using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class Blocks : IResourceModule {
    // REMINDER: since block state properties are unavailable as of right now, keep in mind that block behavior definitions might change when they have actual purpose
    private static int idCounter = 0;
    private static Dictionary<int, Block> blocksById = new();

    public static readonly Block air = InjectID(new GenericBlock("Air", Tile("air"), null));
    public static readonly Block grass = InjectID(new GenericBlock("Grass Block", Tile("grass"), Items.grassBlock));
    public static readonly Block dirt = InjectID(new GenericBlock("Dirt Block", Tile("dirt"), Items.dirtBlock));
    public static readonly Block stone = InjectID(new GenericBlock("Stone Block", Tile("stone"), Items.stoneBlock));
    public static readonly Block wood = InjectID(new GenericBlock("Wood", Tile("wood"), Items.woodBlock));
    public static readonly Block leaves = InjectID(new GenericBlock("Leaves", Tile("leaves"), Items.leavesBlock, _ => BlockBehaviors.DropIfPlayerBroke()));

    public static Block ByID(int id) {
        if (blocksById.TryGetValue(id, out Block block)) {
            return block;
        }
        throw new KeyNotFoundException($"Block ID {id} not found.");
	}

    private static TileBase Tile(string name) {
        return IResourceModule.Resource<TileBase, ResourceGroups.Tiles>(name);
    }

    private static TBlock InjectID<TBlock>(TBlock block) where TBlock : Block {
        block.id = idCounter++;
        blocksById[block.id] = block;
		return block;
	}
}
abstract partial class Block {
    public int id { get; internal set; }
}
