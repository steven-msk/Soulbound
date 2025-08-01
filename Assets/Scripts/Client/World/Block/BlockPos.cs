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

    public static bool operator !=(BlockPos pos1, BlockPos pos2) => !(pos1 == pos2);

    public static bool operator ==(BlockPos pos1, BlockPos pos2) {
        return pos1.x == pos2.x && pos1.y == pos2.y;
    }

    public override bool Equals(object obj) {
        if (obj is BlockPos other) {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 31 + x;
            hash = hash * 31 + y;
            return hash;
        }
    }
}
