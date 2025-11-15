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

public class ChunkBlockPosTests {
    [Test]
    public void Constructor_SetsValues() {
        var pos = new ChunkBlockPos(2, 5, 1);
        Assert.AreEqual(2, pos.x);
        Assert.AreEqual(5, pos.y);
        Assert.AreEqual(1, pos.chunkX);
    }

    [Test]
    public void Equality_Operators_Work() {
        var a = new ChunkBlockPos(1, 2, 3);
        var b = new ChunkBlockPos(1, 2, 3);
        var c = new ChunkBlockPos(1, 2, 4);

        Assert.IsTrue(a == b);
        Assert.IsFalse(a != b);
        Assert.IsTrue(a != c);
        Assert.IsFalse(a == c);
    }

    [Test]
    public void EqualsAndHashCode_Work() {
        var a = new ChunkBlockPos(1, 2, 3);
        var b = new ChunkBlockPos(1, 2, 3);
        var c = new ChunkBlockPos(2, 2, 3);

        Assert.IsTrue(a.Equals(b));
        Assert.IsFalse(a.Equals(c));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Test]
    public void ToString_FormatsCorrectly() {
        var pos = new ChunkBlockPos(3, 4, 2);
        Assert.AreEqual("cx:3, cy:4, c:2", pos.ToString());
    }

    [Test]
    public void Conversion_ToVector2Int() {
        var pos = new ChunkBlockPos(7, 8, 0);
        Vector2Int vec = (Vector2Int)pos;

        Assert.AreEqual(new Vector2Int(7, 8), vec);
    }

    [Test]
    public void ToWorldBlockPos_ComputesCorrectly() {
        const int CHUNK_LENGTH = Level.CHUNK_LENGTH;
        var pos = new ChunkBlockPos(5, 10, 2);

        var worldBlock = pos.ToWorldBlockPos();

        Assert.AreEqual(5 + 2 * CHUNK_LENGTH, worldBlock.x);
        Assert.AreEqual(10, worldBlock.y);
    }

    [Test]
    public void FromBlockPos_ComputesCorrectly() {
        const int CHUNK_LENGTH = Level.CHUNK_LENGTH;
        var blockPos = new BlockPos(34, 7);

        var chunkPos = ChunkBlockPos.FromBlockPos(blockPos);

        int expectedChunkX = Mathf.FloorToInt((float)blockPos.x / CHUNK_LENGTH);
        int expectedLocalX = blockPos.x - expectedChunkX * CHUNK_LENGTH;

        Assert.AreEqual(expectedLocalX, chunkPos.x);
        Assert.AreEqual(blockPos.y, chunkPos.y);
        Assert.AreEqual(expectedChunkX, chunkPos.chunkX);
    }

    [Test]
    public void WorldYToIndex_StaticAndInstance_Agree() {
        var pos = new ChunkBlockPos(1, 20, 0);

        int instanceValue = pos.WorldYToIndex();
        int staticValue = ChunkBlockPos.WorldYToIndex(pos.y);

        Assert.AreEqual(staticValue, instanceValue);
        Assert.AreEqual(pos.y - WorldChunk.maxY, instanceValue);
    }

	[Test]
	public void FromWorld_DelegatesToBlockPos() {
		Level level = new Level(LevelGridContext.FromRuntimePrefabs(), 0);

		var worldPos = new Vector2(18.5f, 6.2f);
		var chunkPos = ChunkBlockPos.FromWorld(worldPos, level);

		Assert.AreEqual(Mathf.FloorToInt(worldPos.y), chunkPos.y);
	}

	[Test]
	public void UnderlyingChunk_ReturnsExpectedChunk() {
		Level level = new Level(LevelGridContext.FromRuntimePrefabs(), 0);
        level.BootstrapWorld(null);

        var pos = new ChunkBlockPos(2, 3, 1);
		var chunk = pos.UnderlyingChunk(level);

		Assert.IsNotNull(chunk);
		Assert.AreEqual(1, chunk.xpos);
	}
}
