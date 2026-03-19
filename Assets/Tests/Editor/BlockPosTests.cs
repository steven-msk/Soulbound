using NUnit.Framework;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.LevelDomain;
using System;
using UnityEngine;

public class BlockPosTests {
	[Test]
	public void EqualityOperators_CheckEqualityCorrectly() {
		var a = new BlockPos(2, 4);
		var b = new BlockPos(2, 4);
		var c = new BlockPos(3, 4);

		Assert.That(a == b, Is.True);
		Assert.That(a != b, Is.False);
		Assert.That(a != c, Is.True);
		Assert.That(a == c, Is.False);
	}

	[Test]
	public void EqualsAndHashCodeMethod_ReturnCorrectValues() {
		var a = new BlockPos(2, 4);
		var b = new BlockPos(2, 4);
		var c = new BlockPos(3, 4);

		Assert.That(a.Equals(b), Is.True);
		Assert.That(a.Equals(c), Is.False);
		Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
	}

	[Test]
	public void ToStringMethod_FormatsCorrectly() {
		var pos = new BlockPos(1, 2);
		Assert.That(pos.ToString(), Is.EqualTo("bx:1, by:2"));
	}

	[Test]
	public void CenterAlignedMethod_ReturnsMiddleOfBlock() {
		var pos = new BlockPos(1, 2);
		var center = pos.GetCenter();
		Assert.That(center, Is.EqualTo(new Vector2(1.5f, 2.5f)));
	}

	[Test]
	public void ConversionOperators_ToAndFromVector2Int_ReturnCorrectValues() {
		var pos = new BlockPos(3, 4);
		var vec = (Vector2Int)pos;
		Assert.That(vec, Is.EqualTo(new Vector2Int(3, 4)));

		var pos2 = (BlockPos)vec;
		Assert.That(pos, Is.EqualTo(pos2));
	}

	[Test]
	public void ConversionOperators_ToAndFromVector2_ReturnCorrectValues() {
		var vec = new Vector2(3.9f, 4.1f);
		var pos = (BlockPos)vec;
		Assert.That(pos, Is.EqualTo(new BlockPos(3, 4)));

		var vec2 = (Vector2)pos;
		Assert.That(vec2, Is.EqualTo(new Vector2(3f, 4f)));
	}

	[Test]
	public void ConversionOperators_ToAndFromVector3_ReturnCorrectValues() {
		var vec = new Vector3(7.9f, 8.1f, 10f);
		var pos = (BlockPos)vec;
		Assert.That(pos, Is.EqualTo(new BlockPos(7, 8)));

		var vec2 = (Vector3)pos;
		Assert.That(vec2, Is.EqualTo(new Vector3(7f, 8f, 0f)));
	}

	[Test]
	public void ConversionOperators_ToAndFromVector3Int_ReturnCorrectValues() {
		var vec = new Vector3Int(9, 10, 11);
		var pos = (BlockPos)vec;
		Assert.That(pos, Is.EqualTo(new BlockPos(9, 10)));

		var vec2 = (Vector3Int)pos;
		Assert.That(vec2, Is.EqualTo(new Vector3Int(9, 10, 0)));
	}

	[Test]
	public void AdditionAndSubtractionOperators_CalculateValuesCorrectly() {
		var pos = new BlockPos(5, 5);

		Assert.That(pos + new Vector2Int(2, 4), Is.EqualTo(new BlockPos(7, 9)));
		Assert.That(pos + (2, 4), Is.EqualTo(new BlockPos(7, 9)));
		Assert.That(pos - new Vector2Int(2, 4), Is.EqualTo(new BlockPos(3, 1)));
		Assert.That(pos - (2, 4), Is.EqualTo(new BlockPos(3, 1)));
	}

	[Test]
	public void MultiplicationAndDivisionOperators_CalculateValuesCorrectly() {
		var pos = new BlockPos(6, 8);

		Assert.That(pos * 2, Is.EqualTo(new BlockPos(12, 16)));
		Assert.That(pos / 2, Is.EqualTo(new BlockPos(3, 4)));
	}

	[Test]
	public void DivisionOperator_DivisionByZeroThrows() {
		var pos = new BlockPos(6, 8);
		Assert.Throws<DivideByZeroException>(() => { var result = pos / 0; });
	}

	[Test]
	public void ToChunkBlockPosMethod_ComputesRelativeToChunk() {
		var blockPos = new BlockPos(-40, 15);
		int chunkX = -2;

		var chunkPos = blockPos.ToChunkPos();

		int expectedLocalX = blockPos.x - chunkX * Level.CHUNK_LENGTH;
		Assert.That(chunkPos.x, Is.EqualTo(expectedLocalX));
		Assert.That(chunkPos.y, Is.EqualTo(blockPos.y));
		Assert.That(chunkPos.chunkX, Is.EqualTo(chunkX));
		Assert.That(Level.ChunkXAt(blockPos.x), Is.EqualTo(chunkX));
	}
}
