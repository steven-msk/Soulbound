using SoulboundEngine.Common.Math;
using SoulboundEngine.World.Chunk;
using System;

namespace SoulboundEngine.World.Block {
	using Level = Level.Level;

	public struct BlockPos {
        public int x;
        public int y;

        public BlockPos(int x, int y) {
            this.x = x;
            this.y = y;
        }

		public static BlockPos From(Vec2d vec) {
			Vec2i floor = vec.FloorToInt();
			return new BlockPos(floor.x, floor.y);
		}

		public static BlockPos From(Vec2i vec) {
			return new BlockPos(vec.x, vec.y);
		}

		public readonly Vec2d ToVec2d() => new(this.x, this.y);
		public readonly Vec2i ToVec2i() => new(this.x, this.y);

		public override readonly string ToString() => $"bx:{this.x},by:{this.y}";

        public readonly ChunkBlockPos ToChunkPos() {
            int cx = Level.ToChunkX(this.x);
            int chunkX = Level.ChunkXAt(this.x);
            return new ChunkBlockPos(cx, this.y, chunkX);
        }

        public static bool operator !=(BlockPos pos1, BlockPos pos2) => !(pos1 == pos2);

        public static bool operator ==(BlockPos pos1, BlockPos pos2) {
            return pos1.x == pos2.x && pos1.y == pos2.y;
        }

        public static BlockPos operator +(BlockPos pos, Vec2i vec) => new(pos.x + vec.x, pos.y + vec.y);

        public static BlockPos operator +(BlockPos pos, (int x, int y) vec) => new(pos.x + vec.x, pos.y + vec.y);

        public static BlockPos operator -(BlockPos pos, Vec2i vec) => new(pos.x - vec.x, pos.y - vec.y);

		public static BlockPos operator -(BlockPos pos, (int x, int y) vec) => new(pos.x - vec.x, pos.y - vec.y);

		public static BlockPos operator *(BlockPos pos, int scalar) => new(pos.x * scalar, pos.y * scalar);

        public static BlockPos operator /(BlockPos pos, int scalar) {
            if (scalar == 0) { 
                throw new DivideByZeroException("Cannot divide BlockPos by zero.");
            }
            return new BlockPos(pos.x / scalar, pos.y / scalar);
        }

        public readonly Vec2d GetCenter() => new(this.x + 0.5f, this.y + 0.5f);

		public override readonly bool Equals(object obj) {
            if (obj is BlockPos other) {
                return this == other;
            }
            return false;
        }

		public static BlockPos Parse(string s) {
			string[] coords = s.Split(',');
			if (coords.Length != 2) throw ParseException(s);

			string bx = coords[0].Replace("bx:", string.Empty);
			string by = coords[1].Replace("by:", string.Empty);

			int x = int.Parse(bx);
			int y = int.Parse(by);
			return new BlockPos(x, y);
		}

		private static ArgumentException ParseException(string s) {
			return new ArgumentException("Could not parse BlockPos: " + s);
		}

        public override readonly int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 31 + this.x;
                hash = hash * 31 + this.y;
                return hash;
            }
        }
    }
}
