using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct ChunkBlockPos {
	public int x;
	public int y;
	public int chunkX;

	public ChunkBlockPos(int x, int y, int chunkX) {
		this.x = x; 
		this.y = y;
		this.chunkX = chunkX;
	}

	public static ChunkBlockPos FromBlockPos(BlockPos blockPos) {
		int chunkX = Mathf.FloorToInt((float)blockPos.x / Level.CHUNK_LENGTH);
		int localX = blockPos.x - chunkX * Level.CHUNK_LENGTH;
		return new ChunkBlockPos(localX, blockPos.y, chunkX);
	}

	public override string ToString() => $"cx:{x}, cy:{y}, c:{chunkX}";

	public BlockPos ToWorldBlockPos() => new BlockPos(this.x + this.chunkX * Level.CHUNK_LENGTH, this.y);

	public int WorldYToIndex() => WorldYToIndex(this.y);

	public static int WorldYToIndex(int worldY) => worldY - WorldChunk.maxY;
}
