using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public static partial class Blocks {
    // REMINDER: since block state properties are unavailable as of right now, keep in mind that block behavior definitions might change when they have actual purpose

    public static readonly Block air = new GenericBlock("Air", AssetRegistry.Get<Tile>("air"), null);
    public static readonly Block grass = new GenericBlock("Grass Block", AssetRegistry.Get<RuleTile>("grass"), Items.grassBlock);
    public static readonly Block dirt = new GenericBlock("Dirt Block", AssetRegistry.Get<Tile>("dirt"), Items.dirtBlock);
    public static readonly Block stone = new GenericBlock("Stone Block", AssetRegistry.Get<Tile>("stone"), Items.stoneBlock);
    public static readonly Block wood = new GenericBlock("Wood", AssetRegistry.Get<Tile>("wood"), Items.woodBlock);
    public static readonly Block leaves = new GenericBlock("Leaves", AssetRegistry.Get<Tile>("leaves"), Items.leavesBlock, _ => BlockBehaviors.DropIfPlayerBroke());
}
