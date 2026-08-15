using SoulboundEngine.Common.Math;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.World.Physics {
	public readonly struct AABB : IEquatable<AABB> {
		private const double EPSILON = 1.0E-7;
		public readonly double minX;
		public readonly double minY;
		public readonly double maxX;
		public readonly double maxY;

		public AABB(double minX, double minY, double maxX, double maxY) {
			this.minX = minX;
			this.minY = minY;
			this.maxX = maxX;
			this.maxY = maxY;
		}

		public AABB(BlockPos pos) 
			: this(pos.x, pos.y, pos.x + 1, pos.y + 1) {
		}

		public AABB(Vec2d min, Vec2d max) 
			: this(min.x, min.y, max.x, max.y) {
		}

		public static AABB UnitSquareFromLowerCorner(Vec2d pos) {
			return new AABB(pos.x, pos.y, pos.x + 1, pos.y + 1);
		}

		public static AABB EncapsulatingFullBlocks(BlockPos posA, BlockPos posB) {
			return new AABB(
				Math.Min(posA.x, posB.x),
				Math.Min(posA.y, posB.y),
				Math.Max(posA.x, posB.x),
				Math.Max(posA.y, posB.y)
			);
		}

		public static AABB OfSize(Vec2d center, double sizeX, double sizeY) {
			return new AABB(center.x - sizeX * 0.5d, center.y - sizeY * 0.5d, center.x + sizeX * 0.5d, center.y + sizeY * 0.5d);
		}

		public AABB SetMinX(double minX) => new(minX, this.minY, this.maxX, this.maxY);
		public AABB SetMinY(double minY) => new(this.minX, minY, this.maxX, this.maxY);

		public AABB SetMaxX(double maxX) => new(this.minX, this.minY, maxX, this.maxY);
		public AABB SetMaxY(double maxY) => new(this.minX, this.minY, this.maxX, maxY);

		public double GetMin(Axis axis) => axis.Get(this.minX, this.minY);
		public double GetMax(Axis axis) => axis.Get(this.maxX, this.maxY);

		public AABB ContractBy(Vec2d delta) => this.ContractBy(delta.x, delta.y);
		public AABB ContractBy(double xa, double ya) {
			double minX = this.minX;
			double minY = this.minY;
			double maxX = this.maxX;
			double maxY = this.maxY;

			if (xa < 0.0d) {
				minX -= xa;
			} else if (xa > 0.0d) {
				maxX -= xa;
			}

			if (ya < 0.0d) {
				minY -= ya;
			} else if (ya > 0.0d) {
				maxY -= ya;
			}

			return new AABB(minX, minY, maxX, maxY);
		}

		public AABB ExpandBy(Vec2d delta) => this.ExpandBy(delta.x, delta.y);
		public AABB ExpandBy(double xa, double ya) {
			double minX = this.minX;
			double minY = this.minY;
			double maxX = this.maxX;
			double maxY = this.maxY;

			if (xa < 0.0d) {
				minX += xa;
			} else if (xa > 0.0d) {
				maxX += xa;
			}

			if (ya < 0.0d) {
				minY += ya;
			} else if (ya > 0.0d) {
				maxY += ya;
			}

			return new AABB(minX, minY, maxX, maxY);
		}

		public AABB Inflate(double amount) => this.Inflate(amount, amount);
		public AABB Inflate(double xAdd, double yAdd) {
			double minX = this.minX - xAdd;
			double minY = this.minY - yAdd;
			double maxX = this.maxX + xAdd;
			double maxY = this.maxY + yAdd;
			return new AABB(minX, minY, maxX, maxY);
		}

		public AABB Deflate(double xSub, double ySub) => this.Inflate(-xSub, -ySub);
		public AABB Deflate(double amount) => this.Inflate(-amount);

		public AABB Intersect(AABB other) {
			double minX = Math.Max(this.minX, other.minX);
			double minY = Math.Max(this.minY, other.minY);
			double maxX = Math.Min(this.maxX, other.maxX);
			double maxY = Math.Min(this.maxY, other.maxY);
			return new AABB(minX, minY, maxX, maxY);
		}

		public AABB Minmax(AABB other) {
			double minX = Math.Min(this.minX, other.minX);
			double minY = Math.Min(this.minY, other.minY);
			double maxX = Math.Max(this.maxX, other.maxX);
			double maxY = Math.Max(this.maxY, other.maxY);
			return new AABB(minX, minY, maxX, maxY);
		}

		public AABB Move(double xa, double ya) {
			return new AABB(this.minX + xa, this.minY + ya, this.maxX + xa, this.maxY + ya);
		}
		public AABB Move(BlockPos pos) {
			return new AABB(this.minX + pos.x, this.minY + pos.y, this.maxX + pos.x, this.maxY + pos.y);
		}

		public AABB Move(Vec2d pos) => this.Move(pos.x, pos.y);
		public AABB Move(Vec2f pos) => this.Move(pos.x, pos.y);

		public bool Intersects(AABB other) {
			return this.Intersects(other.minX, other.minY, other.maxX, other.maxY);
		}
		public bool Intersects(double minX, double minY, double maxX, double maxY) {
			return this.minX < maxX && this.maxX > minX && this.minY < maxY && this.maxY > minY;
		}
		public bool Intersects(Vec2d min, Vec2d max) {
			return this.Intersects(Math.Min(min.x, max.x), Math.Min(min.y, max.y), Math.Max(min.x, max.x), Math.Max(min.y, max.y));
		}
		public bool Intersects(BlockPos pos) {
			return this.Intersects(pos.x, pos.y, pos.x + 1, pos.y + 1);
		}

		public bool Contains(Vec2d pos) => this.Contains(pos.x, pos.y);
		public bool Contains(Vec2f pos) => this.Contains(pos.x, pos.y);
		public bool Contains(Vec2i pos) => this.Contains(pos.x, pos.y);
		public bool Contains(BlockPos pos) => this.Contains(pos.x, pos.y);
		public bool Contains(double x, double y) {
			return x >= this.minX && x < this.maxX && y >= this.minY && y < this.maxY;
		}

		public double GetSize() {
			double sizeX = this.GetXSize();
			double sizeY = this.GetYSize();
			return (sizeX + sizeY) / 2.0d;
		}

		public double GetXSize() => this.maxX - this.minX;
		public double GetYSize() => this.maxY - this.minY;

		public Vec2d? Clip(Vec2d from, Vec2d to) {
			return Clip(this.minX, this.minY, this.maxX, this.maxY, from, to);
		}

		public static Vec2d? Clip(double minX, double minY, double maxX, double maxY, Vec2d from, Vec2d to) {
			double[] scaleReference = new double[] { 1.0d };
			double dx = to.x - from.x;
			double dy = to.y - from.y;
			Direction? direction = GetDirection(minX, minY, maxX, maxY, from, scaleReference, null, dx, dy);
			if (direction == null) return null;

			double scale = scaleReference[0];
			return from.Add(scale * dx, scale * dy);
		}

		private static Direction? GetDirection(AABB aabb, Vec2d from, double[] scaleReference, Direction? direction, double dx, double dy) {
			return GetDirection(aabb.minX, aabb.minY, aabb.maxX, aabb.maxY, from, scaleReference, direction, dx, dy);
		}

		private static Direction? GetDirection(
			double minX,
			double minY,
			double maxX,
			double maxY,
			Vec2d from,
			double[] scaleReference,
			Direction? direction,
			double dx,
			double dy
		) {
			if (dx > EPSILON) {
				direction = ClipPoint(scaleReference, direction, dx, dy, minX, minY, maxY, Direction.Left, from.x, from.y);
			} else if (dx < -EPSILON) {
				direction = ClipPoint(scaleReference, direction, dx, dy, maxX, minY, maxY, Direction.Right, from.x, from.y);
			}

			if (dy > EPSILON) {
				direction = ClipPoint(scaleReference, direction, dy, dx, minY, minX, maxX, Direction.Down, from.y, from.x);
			} else if (dy < -EPSILON) {
				direction = ClipPoint(scaleReference, direction, dy, dx, maxY, minX, maxX, Direction.Up, from.y, from.x);
			}

			return direction;
		}

		private static Direction? ClipPoint(
		    double[] scaleReference,
		    Direction? direction,
		    double da,
		    double db,
		    double point,
		    double minB,
		    double maxB,
		    Direction newDirection,
		    double fromA,
		    double fromB
		) {
			double s = (point - fromA) / da;
			double pb = fromB + s * db;
			if (0.0d < s && s < scaleReference[0] && minB - EPSILON < pb && pb < maxB + EPSILON) {
				scaleReference[0] = s;
				return newDirection;
			} else {
				return direction;
			}
		}

		public bool CollidedAlongVector(Vec2d vector, List<AABB> boxes) {
			Vec2d from = this.GetCenter();
			Vec2d to = from.Add(vector);

			foreach (var aabb in boxes) {
				AABB inflated = aabb.Inflate(this.GetXSize() * 0.5d - EPSILON, this.GetYSize() * 0.5d - EPSILON);
				if (inflated.Contains(to) || inflated.Contains(from)) {
					return true;
				}
				if (inflated.Clip(from, to).HasValue) {
					return true;
				}
			}

			return false;
		}

		public Vec2d GetCenter() {
			return new Vec2d(Maths.Lerp(this.minX, this.maxX, 0.5d), Maths.Lerp(this.minY, this.maxY, 0.5d));
		}

		public Vec2d GetBottomCenter() {
			return new Vec2d(Maths.Lerp(this.minX, this.maxX, 0.5d), this.minY);
		}
		public Vec2d GetLeftCenter() {
			return new Vec2d(this.minX, Maths.Lerp(this.minY, this.maxY, 0.5d));
		}
		public Vec2d GetTopCenter() {
			return new Vec2d(Maths.Lerp(this.minX, this.maxX, 0.5d), this.maxY);
		}
		public Vec2d GetRightCenter() {
			return new Vec2d(this.maxX, Maths.Lerp(this.minY, this.maxY, 0.5d));
		}

		public double SqrDistanceTo(Vec2d point) {
			double dx = Math.Max(Math.Max(this.minX - point.x, point.x - this.maxX), 0.0);
			double dy = Math.Max(Math.Max(this.minY - point.y, point.y - this.maxY), 0.0);
			return Maths.LengthSqr(dx, dy);
		}
		public double SqrDistanceTo(AABB other) {
			double dx = Math.Max(Math.Max(this.minX - other.maxX, other.minX - this.maxX), 0.0);
			double dy = Math.Max(Math.Max(this.minY - other.maxY, other.minY - this.maxY), 0.0);
			return Maths.LengthSqr(dx, dy);
		}

		public Vec2d GetMin() => new(this.minX, this.minY);
		public Vec2d GetMax() => new(this.maxX, this.maxY);

		public bool HasNaN() {
			return double.IsNaN(this.minX)
				|| double.IsNaN(this.minY)
				|| double.IsNaN(this.maxX)
				|| double.IsNaN(this.maxY);
		}

		public override int GetHashCode() {
			const int prime = 31;
			long temp = BitConverter.DoubleToInt64Bits(this.minX);
			int result = (int)(temp ^ temp >> 32);
			temp = BitConverter.DoubleToInt64Bits(this.minY);
			result = prime * result + (int)(temp ^ temp >> 32);
			temp = BitConverter.DoubleToInt64Bits(this.maxX);
			result = prime * result + (int)(temp ^ temp >> 32);
			temp = BitConverter.DoubleToInt64Bits(this.maxY);
			result = prime * result + (int)(temp ^ temp >> 32);
			return result;
		}

		public override bool Equals(object obj) {
			return obj is AABB other && this.Equals(other);
		}

		public bool Equals(AABB other) {
			return this.minX == other.minX && this.minY == other.minY
				&& this.maxX == other.maxX && this.maxY == other.maxY;
		}

		public override string ToString() {
			return $"AABB[{this.minX},{this.minY};{this.maxX},{this.maxY}]";
		}

		public sealed class Builder {
			private double minX = float.PositiveInfinity;
			private double minY = float.PositiveInfinity;
			private double maxX = float.NegativeInfinity;
			private double maxY = float.NegativeInfinity;

			public bool isDefined { get; private set; }
			
			public void Include(Vec2d pos) {
				this.minX = Math.Min(this.minX, pos.x);
				this.minY = Math.Min(this.minY, pos.y);
				this.maxX = Math.Max(this.maxX, pos.x);
				this.maxY = Math.Max(this.maxY, pos.y);
				this.isDefined = true;
			}

			public AABB Build() {
				if (!this.isDefined) throw new InvalidOperationException("Cannot build an undefined AABB. Include at least one point");
				return new AABB(this.minX, this.minY, this.maxX, this.maxY);
			}
		}
	}
}
