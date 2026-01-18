using NUnit.Framework;
using SoulboundBackend.Client.World;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WorldTests;

public class BlockPosTests {
    [Test]
    public void Constructor_SetsValues() {
        var pos = new BlockPos(3, 5);
        Assert.AreEqual(3, pos.x);
        Assert.AreEqual(5, pos.y);
    }

    [Test]
    public void Equality_Operators_Work() {
        var a = new BlockPos(2, 4);
        var b = new BlockPos(2, 4);
        var c = new BlockPos(3, 4);

        Assert.IsTrue(a == b);
        Assert.IsFalse(a != b);
        Assert.IsTrue(a != c);
        Assert.IsFalse(a == c);
    }

    [Test]
    public void EqualsAndHashCode_Work() {
        var a = new BlockPos(2, 4);
        var b = new BlockPos(2, 4);
        var c = new BlockPos(3, 4);

        Assert.IsTrue(a.Equals(b));
        Assert.IsFalse(a.Equals(c));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Test]
    public void ToString_FormatsCorrectly() {
        var pos = new BlockPos(1, 2);
        Assert.AreEqual("bx:1, by:2", pos.ToString());
    }

    [Test]
    public void CenterAligned_ReturnsMiddleOfBlock() {
        var pos = new BlockPos(1, 2);
        var center = pos.CenterAligned();
        Assert.AreEqual(new Vector2(1.5f, 2.5f), center);
    }

    [Test]
    public void Conversion_ToAndFrom_Vector2Int() {
        var pos = new BlockPos(3, 4);
        var vec = (Vector2Int)pos;
        Assert.AreEqual(new Vector2Int(3, 4), vec);

        var pos2 = (BlockPos)vec;
        Assert.AreEqual(pos, pos2);
    }

    [Test]
    public void Conversion_ToAndFrom_Vector2_FloorsCorrectly() {
        var vec = new Vector2(3.9f, 4.1f);
        var pos = (BlockPos)vec;
        Assert.AreEqual(new BlockPos(3, 4), pos);

        var vec2 = (Vector2)pos;
        Assert.AreEqual(new Vector2(3, 4), vec2);
    }

    [Test]
    public void Conversion_ToAndFrom_Vector3_FloorsCorrectly() {
        var vec = new Vector3(7.9f, 8.1f, 10f);
        var pos = (BlockPos)vec;
        Assert.AreEqual(new BlockPos(7, 8), pos);

        var vec2 = (Vector3)pos;
        Assert.AreEqual(new Vector3(7, 8, 0f), vec2);
    }

    [Test]
    public void Conversion_ToAndFrom_Vector3Int() {
        var vec = new Vector3Int(9, 10, 11);
        var pos = (BlockPos)vec;
        Assert.AreEqual(new BlockPos(9, 10), pos);

        var vec2 = (Vector3Int)pos;
        Assert.AreEqual(new Vector3Int(9, 10, 0), vec2);
    }

    [Test]
    public void Operators_AdditionAndSubtraction() {
        var pos = new BlockPos(5, 5);

        Assert.AreEqual(new BlockPos(7, 9), pos + new Vector2Int(2, 4));
        Assert.AreEqual(new BlockPos(7, 9), pos + (2, 4));
        Assert.AreEqual(new BlockPos(3, 1), pos - new Vector2Int(2, 4));
        Assert.AreEqual(new BlockPos(3, 1), pos - (2, 4));
    }

    [Test]
    public void Operators_MultiplicationAndDivision() {
        var pos = new BlockPos(6, 8);

        Assert.AreEqual(new BlockPos(12, 16), pos * 2); 
        Assert.AreEqual(new BlockPos(3, 4), pos / 2);
    }

    [Test]
    public void Division_ByZero_Throws() {
        var pos = new BlockPos(6, 8);
        Assert.Throws<DivideByZeroException>(() => { var result = pos / 0; });
    }

	[Test]
	public void ToChunkBlockPos_ComputesRelativeToChunk() {
		var blockPos = new BlockPos(-40, 15);
		int chunkX = -2;

		var chunkPos = blockPos.ToChunk();

        int expectedLocalX = blockPos.x - chunkX * Level.CHUNK_LENGTH;
		Assert.AreEqual(expectedLocalX, chunkPos.x);
		Assert.AreEqual(blockPos.y, chunkPos.y);
        Assert.AreEqual(chunkX, chunkPos.chunkX);
        Assert.AreEqual(chunkX, Level.ChunkXAt(blockPos.x));
	}
}
