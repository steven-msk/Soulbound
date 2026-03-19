using NUnit.Framework;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.LevelDomain;
using UnityEngine;

public class ChunkBlockPosTests {
    [Test]
    public void Constructor_SetsValues() {
        var pos = new ChunkBlockPos(2, 5, 1);
        Assert.That(pos.x, Is.EqualTo(2));
        Assert.That(pos.y, Is.EqualTo(5));
        Assert.That(pos.chunkX, Is.EqualTo(1));
    }

    [Test]
    public void Equality_Operators_Work() {
        var a = new ChunkBlockPos(1, 2, 3);
        var b = new ChunkBlockPos(1, 2, 3);
        var c = new ChunkBlockPos(1, 2, 4);

        Assert.That(a == b, Is.True);
        Assert.That(a != b, Is.False);
		Assert.That(a != c, Is.True);
        Assert.That(a == c, Is.False);
    }

    [Test]
    public void EqualsAndHashCode_Work() {
        var a = new ChunkBlockPos(1, 2, 3);
        var b = new ChunkBlockPos(1, 2, 3);
        var c = new ChunkBlockPos(2, 2, 3);

        Assert.That(a.Equals(b), Is.True);
        Assert.That(a.Equals(c), Is.False);
        Assert.That(b.GetHashCode(), Is.EqualTo(a.GetHashCode()));
    }

    [Test]
    public void ToString_FormatsCorrectly() {
        var pos = new ChunkBlockPos(3, 4, 2);
        Assert.That(pos.ToString(), Is.EqualTo("cx:3, cy:4, c:2"));
    }

    [Test]
    public void Conversion_ToVector2Int() {
        var pos = new ChunkBlockPos(7, 8, 0);
        Vector2Int vec = (Vector2Int)pos;

        Assert.That(vec, Is.EqualTo(new Vector2Int(7, 8)));
    }

    [Test]
    public void ToWorldBlockPos_ComputesCorrectly() {
        const int CHUNK_LENGTH = Level.CHUNK_LENGTH;
        var pos = new ChunkBlockPos(5, 10, 2);

        var worldBlock = pos.ToBlock();

        Assert.That(worldBlock.x, Is.EqualTo(5 + 2 * CHUNK_LENGTH));
        Assert.That(worldBlock.y, Is.EqualTo(10));
    }

    [Test]
    public void FromBlockPos_ComputesCorrectly() {
        const int CHUNK_LENGTH = Level.CHUNK_LENGTH;
        var blockPos = new BlockPos(34, 7);

        var chunkPos = ChunkBlockPos.FromBlockPos(blockPos);

        int expectedChunkX = Mathf.FloorToInt((float)blockPos.x / CHUNK_LENGTH);
        int expectedLocalX = blockPos.x - expectedChunkX * CHUNK_LENGTH;

        Assert.That(chunkPos.x, Is.EqualTo(expectedLocalX));
        Assert.That(chunkPos.y, Is.EqualTo(blockPos.y));
        Assert.That(chunkPos.chunkX, Is.EqualTo(expectedChunkX));
    }

    [Test]
    public void WorldYToIndex_StaticAndInstance_Agree() {
        var pos = new ChunkBlockPos(1, 20, 0);

        int instanceValue = pos.WorldYToIndex();
        int staticValue = ChunkBlockPos.WorldYToIndex(pos.y);

        Assert.That(instanceValue, Is.EqualTo(staticValue));
        Assert.That(instanceValue, Is.EqualTo(pos.y - WorldChunk.maxY));
    }

	[Test]
	public void FromWorld_DelegatesToBlockPos() {
		var worldPos = new Vector2(18.5f, 6.2f);
		var chunkPos = ChunkBlockPos.FromWorld(worldPos);

		Assert.That(chunkPos.y, Is.EqualTo(Mathf.FloorToInt(worldPos.y)));
	}
}
