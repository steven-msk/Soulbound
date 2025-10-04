using NUnit.Framework;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Structure;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StructurePlacementTests {
    [SetUp]
    public void Setup() {
        SoulboundBackend.Core.Resource.ResourceGroups.Bootstrap();
        StaticResetManager.ResetAll();
    }

    [Test]
    public void Equality_WorksForSameData() {
        var origin = new ChunkBlockPos(1, 2, 3);
        var bounds = new BoundsInt2D(new Vector2Int(0, 0), new Vector2Int(10, 10));
        var dict = new Dictionary<ChunkBlockPos, BlockState> {
            { new ChunkBlockPos(0,0,0), Blocks.stone.defaultState }
        };

        var p1 = new StructurePlacement(origin, "test", dict, bounds);
        var p2 = new StructurePlacement(origin, "test", dict, bounds);

        Assert.IsTrue(p1 == p2);
        Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
    }

    [Test]
    public void PersistentExistence_ReturnsTrue_IfStateOverridesNotEmpty() {
        var placement = new StructurePlacement(
            new ChunkBlockPos(1, 2, 3), "id",
            new Dictionary<ChunkBlockPos, BlockState> {
                { new ChunkBlockPos(0,0,0), Blocks.stone.defaultState }
            },
            new BoundsInt2D(Vector2Int.zero, Vector2Int.one)
        );
        Assert.IsTrue(placement.PersistentExistence());
    }

    [Test]
    public void PersistentExistence_ReturnsFalse_IfStateOverridesEmpty() {
        var placement = new StructurePlacement(
            new ChunkBlockPos(1, 2, 3), "id",
            new Dictionary<ChunkBlockPos, BlockState>(),
            new BoundsInt2D(Vector2Int.zero, Vector2Int.one)
        );
        Assert.IsFalse(placement.PersistentExistence());
    }
}
