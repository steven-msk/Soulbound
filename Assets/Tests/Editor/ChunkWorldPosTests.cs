using NUnit.Framework;
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChunkWorldPosTests {
    [Test]
    public void EqualityOperators_ReturnCorrectValues() {
        var a = new ChunkWorldPos(1.5f, 2.5f, 3);
        var b = new ChunkWorldPos(1.5f, 2.5f, 3);
        var c = new ChunkWorldPos(1.5f, 2.5f, 4);

        Assert.That(a == b, Is.True);
        Assert.That(a != b, Is.False);
        Assert.That(a != c, Is.True);
        Assert.That(a == c, Is.False);
    }

    [Test]
    public void EqualsAndHashCodeMethods_ReturnCorrectValues() {
        var a = new ChunkWorldPos(1.0f, 2.0f, 3);
        var b = new ChunkWorldPos(1.0f, 2.0f, 3);
        var c = new ChunkWorldPos(2.0f, 2.0f, 3);

        Assert.That(a.Equals(b), Is.True);
        Assert.That(a.Equals(c), Is.False);
        Assert.That(b.GetHashCode(), Is.EqualTo(a.GetHashCode()));
    }

    [Test]
    public void ToStringMethod_FormatsValueCorrectly() {
        var pos = new ChunkWorldPos(3.25f, 4.75f, 2);
        Assert.That(pos.ToString(), Is.EqualTo("cwx:3.25, cwy:4.75, c:2"));
    }

    [Test]
    public void FromBlockPosMethod_CalculatesValueCorrectly() {
        const int CHUNK_LENGTH = Level.CHUNK_LENGTH;
        var blockPos = new BlockPos(34, 7);

        var worldPos = ChunkWorldPos.FromBlockPos(blockPos);

        int expectedChunkX = Mathf.FloorToInt((float)blockPos.x / CHUNK_LENGTH);
        float expectedLocalX = blockPos.x - expectedChunkX * CHUNK_LENGTH;

        Assert.That(worldPos.x, Is.EqualTo(expectedLocalX));
        Assert.That(worldPos.y, Is.EqualTo(blockPos.y));
        Assert.That(worldPos.chunkX, Is.EqualTo(expectedChunkX));
    }

	[Test]
	public void FromWorldMethod_CalculatesChunkXCorrectly() {
		var worldPos = new Vector2(40.2f, 15.8f);
		var chunkWorld = ChunkWorldPos.FromWorld(worldPos);

		Assert.That(chunkWorld.chunkX, Is.EqualTo(Mathf.FloorToInt(worldPos.x / Level.CHUNK_LENGTH)));
		Assert.That(chunkWorld.x, Is.EqualTo(worldPos.x));
		Assert.That(chunkWorld.y, Is.EqualTo(worldPos.y));
	}
}
