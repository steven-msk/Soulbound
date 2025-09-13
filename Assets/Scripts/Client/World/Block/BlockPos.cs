using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Core;
using System;
using UnityEngine;

namespace SoulboundBackend.Client.World {
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

        public static bool operator !=(BlockPos pos1, BlockPos pos2) => !(pos1 == pos2);

        public static bool operator ==(BlockPos pos1, BlockPos pos2) {
            return pos1.x == pos2.x && pos1.y == pos2.y;
        }

        public static explicit operator Vector2Int(BlockPos pos) => new Vector2Int(pos.x, pos.y);

        public static explicit operator BlockPos(Vector2Int vec) => new BlockPos(vec.x, vec.y);

        public static explicit operator Vector2(BlockPos pos) => new Vector2(pos.x, pos.y);

        public static explicit operator BlockPos(Vector2 vec) => new BlockPos(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));

        public static explicit operator Vector3(BlockPos pos) => new Vector3(pos.x, pos.y, 0f);

        public static explicit operator BlockPos(Vector3 vec) => new BlockPos(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));

        public static explicit operator Vector3Int(BlockPos pos) => new Vector3Int(pos.x, pos.y, 0);

        public static explicit operator BlockPos(Vector3Int vec) => new BlockPos(vec.x, vec.y);

        public static BlockPos operator +(BlockPos pos, Vector2Int vec) => new BlockPos(pos.x + vec.x, pos.y + vec.y);

        public static BlockPos operator +(BlockPos pos, (int x, int y) vec) => new BlockPos(pos.x + vec.x, pos.y + vec.y);

        public static BlockPos operator -(BlockPos pos, Vector2Int vec) => new BlockPos(pos.x - vec.x, pos.y - vec.y);

        public static BlockPos operator -(BlockPos pos, (int x, int y) vec) => new BlockPos(pos.x - vec.x, pos.y - vec.y);

        public static BlockPos operator *(BlockPos pos, int scalar) => new BlockPos(pos.x * scalar, pos.y * scalar);

        public static BlockPos operator /(BlockPos pos, int scalar) {
            if (scalar == 0) { 
                throw new DivideByZeroException("Cannot divide BlockPos by zero.");
            }
            return new BlockPos(pos.x / scalar, pos.y / scalar);
        }

        public Vector2 CenterAligned() => new Vector2(x + 0.5f, y + 0.5f);

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
}
