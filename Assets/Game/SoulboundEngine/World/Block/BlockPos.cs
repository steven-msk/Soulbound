namespace SoulboundEngine.World.Block {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Physics;
	using System;
	using System.Collections.Generic;

	public readonly struct BlockPos {
		public const int BOTTOM_LEFT_CORNER = 0;
		public const int BOTTOM_RIGHT_CORNER = 1;
		public const int TOP_LEFT_CORNER = 2;
		public const int TOP_RIGHT_CORNER = 3;
        public readonly int x;
        public readonly int y;

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

		public Vec2d ToVec2d() => new(this.x, this.y);
		public Vec2i ToVec2i() => new(this.x, this.y);

		public override string ToString() => $"bx:{this.x},by:{this.y}";

		public ChunkBlockPos ToChunkPos() => ChunkBlockPos.FromBlockPos(this);

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
			return scalar == 0
				? throw new DivideByZeroException("Cannot divide BlockPos by zero.")
				: new BlockPos(pos.x / scalar, pos.y / scalar);
		}

		public Vec2d GetCenter() => new(this.x + 0.5d, this.y + 0.5d);

		public Vec2d GetBottomCenter() => new(this.x + 0.5d, this.y);

		public BlockPos AtY(int y) => new(this.x, y);

		public Vec2d GetCorner(int corner) {
			return corner switch {
				BOTTOM_LEFT_CORNER => new Vec2d(this.x, this.y),
				BOTTOM_RIGHT_CORNER => new Vec2d(this.x + 1.0d, this.y),
				TOP_LEFT_CORNER => new Vec2d(this.x, this.y + 1.0d),
				TOP_RIGHT_CORNER => new Vec2d(this.x + 1.0d, this.y + 1.0d),
				_ => throw new ArgumentException()
			};
		}

		public double GetMinX() => this.x;
		public double GetMaxX() => this.x + 1.0d;

		public double GetMinY() => this.y;
		public double GetMaxY() => this.y + 1.0d;

		public BlockPos Offset(int x, int y) {
			return new BlockPos(this.x + x, this.y + y);
		}

        public BlockPos Down(int amount = 1) => this.Offset(0, -amount);

        public BlockPos Up(int amount = 1) => this.Offset(0, amount);

        public BlockPos Left(int amount = 1) => this.Offset(-amount, 0);

        public BlockPos Right(int amount = 1) => this.Offset(amount, 0);

		public BlockPos Multiply(int scale) {
			return new BlockPos(this.x * scale, this.y * scale);
		}

		public BlockPos Relative(Direction direction, int amount) {
			Vec2i v = direction.AsVec2i();
			return new BlockPos(this.x + amount * v.x, this.y + amount * v.y);
		}

		public BlockPos Relative(Axis axis, int amount) {
			int xAmount = axis.Is(Axis.X) ? amount : 0;
			int yAmount = axis.Is(Axis.Y) ? amount : 0;
			return new BlockPos(this.x + xAmount, this.y + yAmount);
		}

		public static IEnumerable<BlockPos> Inside(AABB box) {
			return Inside(Maths.FloorToInt(box.minX), Maths.FloorToInt(box.minY), Maths.FloorToInt(box.maxX), Maths.FloorToInt(box.maxY));
		}

		public static IEnumerable<BlockPos> Inside(int minX, int minY, int maxX, int maxY) {
			int width = maxX - minX + 1;
			int height = maxY - minY + 1;
			int end = width * height;
			int index = 0;
			Mutable pos = new();
			
			while (index != end) {
				int x = index % width;
				int y = index / height;
				index++;
				yield return pos.Set(minX + x, minY + y);
			}
		}

		public override bool Equals(object obj) {
			return obj is BlockPos other && this == other;
		}

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 31 + this.x;
                hash = hash * 31 + this.y;
                return hash;
            }
        }

		public class Mutable {
			private BlockPos blockPos;

			public Mutable() 
				: this(0, 0) {
			}

			public Mutable(double x, double y)
				: this(Maths.FloorToInt(x), Maths.FloorToInt(y)) {
			}

			public Mutable(int x, int y) {
				this.blockPos = new BlockPos(x, y);
			}

			public Mutable(BlockPos pos) {
				this.blockPos = pos;
			}

			public int X => this.blockPos.x;
			public int Y => this.blockPos.y;

			public int GetX() => this.blockPos.x;

			public int GetY() => this.blockPos.y;

			public BlockPos Offset(int x, int y) {
				return this.blockPos.Offset(x, y);
			}

			public BlockPos Multiply(int scale) {
				return this.blockPos.Multiply(scale);
			}

			public BlockPos Relative(Direction direction, int amount) {
				return this.blockPos.Relative(direction, amount);
			}

			public BlockPos Relative(Axis axis, int amount) {
				return this.blockPos.Relative(axis, amount);
			}

			public Mutable Set(int x, int y) {
				this.blockPos = new BlockPos(x, y);
				return this;
			}

			public Mutable Set(double x, double y) {
				return this.Set(Maths.FloorToInt(x), Maths.FloorToInt(y));
			}

			public Mutable Set(Vec2i vec) => this.Set(vec.x, vec.y);

			public Mutable SetWithOffset(Vec2i pos, Direction direction) {
				return this.SetWithOffset(pos, direction.AsVec2i());
			}

			public Mutable SetWithOffset(Vec2i pos, Vec2i offset) {
				return this.Set(pos.x + offset.x, pos.y + offset.y);
			}

			public Mutable Move(Direction direction) => this.Move(direction, 1);

			public Mutable Move(Direction direction, int steps) {
				Vec2i v = direction.AsVec2i();
				return this.Set(this.X + v.x * steps, this.Y + v.y * steps);
			}

			public Mutable Move(int x, int y) {
				return this.Set(this.X + x, this.Y + y);
			}

			public Mutable Move(Vec2i pos) {
				return this.Set(this.X + pos.x, this.Y + pos.y);
			}

			public Mutable Clamp(Axis axis, int min, int max) {
				return axis.Is(Axis.X) ? this.Set(Math.Clamp(this.X, min, max), this.Y)
					: axis.Is(Axis.Y) ? this.Set(this.X, Math.Clamp(this.Y, min, max)) 
						: throw new ArgumentException("Unknown axis: " + axis);
			}

			public Mutable SetX(int x) {
				this.blockPos = new BlockPos(x, this.Y);
				return this;
			}

			public Mutable SetY(int y) {
				this.blockPos = new BlockPos(this.X, y);
				return this;
			}

			public BlockPos Get() => this.blockPos;

			public static implicit operator BlockPos(Mutable mutable) => mutable.blockPos;
		}
    }
}
