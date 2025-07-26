using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct BlockPos {
	public int x;
	public int y;

	public BlockPos(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static BlockPos FromWorld(Vector2 worldPos) => GameManager.instance.Level.ToBlockPos(worldPos);

	public override string ToString() => $"bx:{x}, by:{y}";

	public ChunkBlockPos ToChunkBlockPos(int chunkX) {
		Level level = GameManager.instance.Level;
		int chunkBlockX = Mathf.FloorToInt(this.x - (chunkX * Level.CHUNK_LENGTH));
		return new ChunkBlockPos(chunkBlockX, this.y, chunkX);
	}

	public Vector2Int AsVector() => new Vector2Int(this.x, this.y);
}
