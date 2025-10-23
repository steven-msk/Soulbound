using NUnit.Framework;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChunkWorldPosTests {
    [Test]
    public void Constructor_SetsValues() {
        var pos = new ChunkWorldPos(2.5f, 5.25f, 1);
        Assert.AreEqual(2.5f, pos.x);
        Assert.AreEqual(5.25f, pos.y);
        Assert.AreEqual(1, pos.chunkX);
    }

    [Test]
    public void Equality_Operators_Work() {
        var a = new ChunkWorldPos(1.5f, 2.5f, 3);
        var b = new ChunkWorldPos(1.5f, 2.5f, 3);
        var c = new ChunkWorldPos(1.5f, 2.5f, 4);

        Assert.IsTrue(a == b);
        Assert.IsFalse(a != b);
        Assert.IsTrue(a != c);
        Assert.IsFalse(a == c);
    }

    [Test]
    public void EqualsAndHashCode_Work() {
        var a = new ChunkWorldPos(1.0f, 2.0f, 3);
        var b = new ChunkWorldPos(1.0f, 2.0f, 3);
        var c = new ChunkWorldPos(2.0f, 2.0f, 3);

        Assert.IsTrue(a.Equals(b));
        Assert.IsFalse(a.Equals(c));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Test]
    public void ToString_FormatsCorrectly() {
        var pos = new ChunkWorldPos(3.25f, 4.75f, 2);
        Assert.AreEqual("cwx:3.25, cwy:4.75, c:2", pos.ToString());
    }

    [Test]
    public void FromBlockPos_ComputesCorrectly() {
        const int CHUNK_LENGTH = Level.CHUNK_LENGTH;
        var blockPos = new BlockPos(34, 7);

        var worldPos = ChunkWorldPos.FromBlockPos(blockPos);

        int expectedChunkX = Mathf.FloorToInt((float)blockPos.x / CHUNK_LENGTH);
        float expectedLocalX = blockPos.x - expectedChunkX * CHUNK_LENGTH;

        Assert.AreEqual(expectedLocalX, worldPos.x);
        Assert.AreEqual(blockPos.y, worldPos.y);
        Assert.AreEqual(expectedChunkX, worldPos.chunkX);
    }

	[Test]
	public void FromWorld_AssignsChunkX() {
		var worldPos = new Vector2(40.2f, 15.8f);
		var chunkWorld = ChunkWorldPos.FromWorld(worldPos);

		Assert.AreEqual(Mathf.FloorToInt(worldPos.x / Level.CHUNK_LENGTH), chunkWorld.chunkX);
		Assert.AreEqual(worldPos.x, chunkWorld.x);
		Assert.AreEqual(worldPos.y, chunkWorld.y);
	}

	[Test]
	public void UnderlyingChunk_ReturnsExpectedChunk() {
		Level level = new Level(null, LevelGridContext.FromRuntimePrefabs(), 0);
		level.BootstrapWorld(null);

		var pos = new ChunkWorldPos(3.3f, 4.4f, 2);

		var chunk = pos.UnderlyingChunk(level);

		Assert.IsNotNull(chunk);
		Assert.AreEqual(2, chunk.xpos);
	}
}
