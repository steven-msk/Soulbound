using NUnit.Framework;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using WorldTests;

public class Integrated_ChunkWorldPosTests {
    [Test]
    public void FromWorld_AssignsChunkX() {
        World.CreateAnonymousContext(null, out var worldManager);

        var worldPos = new Vector2(40.2f, 15.8f);
        var chunkWorld = ChunkWorldPos.FromWorld(worldPos);

        Assert.AreEqual(Mathf.FloorToInt(worldPos.x / Level.CHUNK_LENGTH), chunkWorld.chunkX);
        Assert.AreEqual(worldPos.x, chunkWorld.x);
        Assert.AreEqual(worldPos.y, chunkWorld.y);
    }

    [Test]
    public void UnderlyingChunk_ReturnsExpectedChunk() {
        World.CreateAnonymousContext(null, out var worldManager);

        var pos = new ChunkWorldPos(3.3f, 4.4f, 2);

        var chunk = pos.UnderlyingChunk(worldManager.activeLevelManager.Level);

        Assert.IsNotNull(chunk);
        Assert.AreEqual(2, chunk.xpos);
    }
}
