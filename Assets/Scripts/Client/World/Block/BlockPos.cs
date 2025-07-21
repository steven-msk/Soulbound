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

	public override string ToString() => $"bx:{x}, by:{y}";

	public ChunkBlockPos ToChunkBlockPos(int chunkX) {
		Level level = GameManager.instance.Level;
		int chunkBlockX = Mathf.FloorToInt(this.x - (chunkX * Level.CHUNK_LENGTH));
		return new ChunkBlockPos(chunkBlockX, Mathf.FloorToInt(this.y), chunkX);
	}

	public Vector2Int AsVector() => new Vector2Int(this.x, this.y);
}
