using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.World.BlockSystem {
    public struct BlockPos {
        public int x;
        public int y;

        public BlockPos(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override readonly string ToString() => $"bx:{x}, by:{y}";

        public readonly ChunkBlockPos ToChunkPos() {
            int cx = Level.ToChunkX(x);
            int chunkX = Level.ChunkXAt(x);
            return new ChunkBlockPos(cx, this.y, chunkX);
        }

        public static bool operator !=(BlockPos pos1, BlockPos pos2) => !(pos1 == pos2);

        public static bool operator ==(BlockPos pos1, BlockPos pos2) {
            return pos1.x == pos2.x && pos1.y == pos2.y;
        }

        public static explicit operator Vector2Int(BlockPos pos) => new(pos.x, pos.y);

        public static explicit operator BlockPos(Vector2Int vec) => new(vec.x, vec.y);

        public static explicit operator Vector2(BlockPos pos) => new(pos.x, pos.y);

		public static explicit operator BlockPos(Vector2 vec) => new(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));

		public static explicit operator Vector3(BlockPos pos) => new(pos.x, pos.y, 0f);

		public static explicit operator BlockPos(Vector3 vec) => new(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));

        public static explicit operator Vector3Int(BlockPos pos) => new(pos.x, pos.y, 0);

		public static explicit operator BlockPos(Vector3Int vec) => new(vec.x, vec.y);

        public static BlockPos operator +(BlockPos pos, Vector2Int vec) => new(pos.x + vec.x, pos.y + vec.y);

        public static BlockPos operator +(BlockPos pos, (int x, int y) vec) => new(pos.x + vec.x, pos.y + vec.y);

        public static BlockPos operator -(BlockPos pos, Vector2Int vec) => new(pos.x - vec.x, pos.y - vec.y);

		public static BlockPos operator -(BlockPos pos, (int x, int y) vec) => new(pos.x - vec.x, pos.y - vec.y);

		public static BlockPos operator *(BlockPos pos, int scalar) => new(pos.x * scalar, pos.y * scalar);

        public static BlockPos operator /(BlockPos pos, int scalar) {
            if (scalar == 0) { 
                throw new DivideByZeroException("Cannot divide BlockPos by zero.");
            }
            return new BlockPos(pos.x / scalar, pos.y / scalar);
        }

        public readonly Vector2 GetCenter() => new(x + 0.5f, y + 0.5f);

		public override readonly bool Equals(object obj) {
            if (obj is BlockPos other) {
                return this == other;
            }
            return false;
        }

        public override readonly int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 31 + x;
                hash = hash * 31 + y;
                return hash;
            }
        }
    }
}
