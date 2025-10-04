using NUnit.Framework;
using SoulboundBackend.Client.World;
using SoulboundBackend.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using WorldTests;

public class Integrated_BlockPosTests {
    [Test]
    public void FromWorld_UsesLevelConversion() {
        World.CreateAnonymousContext(null, out var worldManager);

        var worldPos = new Vector2(12.3f, 7.7f);
        var blockPos = BlockPos.FromWorld(worldPos);

        Assert.AreEqual(Mathf.FloorToInt(worldPos.x), blockPos.x);
        Assert.AreEqual(Mathf.FloorToInt(worldPos.y), blockPos.y);
    }

    [Test]
    public void ToChunkBlockPos_ComputesRelativeToChunk() {
        World.CreateAnonymousContext(null, out var worldManager);

        var blockPos = new BlockPos(20, 15);
        int chunkX = 1;

        var chunkPos = blockPos.ToChunkBlockPos(chunkX);

        int expectedLocalX = blockPos.x - chunkX * Level.CHUNK_LENGTH;
        Assert.AreEqual(expectedLocalX, chunkPos.x);
        Assert.AreEqual(blockPos.y, chunkPos.y);
        Assert.AreEqual(chunkX, chunkPos.chunkX);
    }
}
