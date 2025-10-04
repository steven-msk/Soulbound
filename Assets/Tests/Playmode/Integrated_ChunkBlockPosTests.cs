using NUnit.Framework;
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

public class Integrated_ChunkBlockPosTests {
    [Test]
    public void FromWorld_DelegatesToBlockPos() {
        World.CreateAnonymousContext(null, out var worldManager);

        var worldPos = new Vector2(18.5f, 6.2f);
        var chunkPos = ChunkBlockPos.FromWorld(worldPos);

        Assert.AreEqual(Mathf.FloorToInt(worldPos.y), chunkPos.y);
    }

    [Test]
    public void UnderlyingChunk_ReturnsExpectedChunk() {
        World.CreateAnonymousContext(null, out var worldManager);

        var pos = new ChunkBlockPos(2, 3, 1);

        var chunk = pos.underlyingChunk;

        Assert.IsNotNull(chunk);
        Assert.AreEqual(1, chunk.xpos);
    }
}
