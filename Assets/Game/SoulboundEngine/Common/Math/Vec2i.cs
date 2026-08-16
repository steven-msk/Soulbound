using System;

namespace SoulboundEngine.Common.Math {
	using Math = System.Math;

	public struct Vec2i : IVec2i, IEquatable<Vec2i> {
		public static readonly Vec2i ZERO = new(0, 0);
		public static readonly Vec2i ONE = new(1, 1);
		public static readonly Vec2i UNIT_X = new(1, 0);
		public static readonly Vec2i UNIT_Y = new(0, 1);
		public static readonly Vec2i MAX = new(int.MaxValue, int.MaxValue);
		public static readonly Vec2i MIN = new(int.MinValue, int.MinValue);
		public int x;
		public int y;

		public Vec2i(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public Vec2i(int s) {
			this.x = s;
			this.y = s;
		}

		readonly int IVec2i.X => this.x;
		readonly int IVec2i.Y => this.y;

		public readonly Vec2f ToVec2f() => new(this.x, this.y);

		public readonly Vec2d ToVec2d() => new(this.x, this.y);

		public Vec2i Set(int s) {
			this.x = s;
			this.y = s;
			return this;
		}

		public Vec2i Set(int x, int y) {
			this.x = x;
			this.y = y;
			return this;
		}

		public Vec2i Set<T>(T v) where T : IVec2i {
			this.x = v.X;
			this.y = v.Y;
			return this;
		}

		public Vec2i Set(Vec2f v) {
			this.x = (int)v.x;
			this.y = (int)v.y;
			return this;
		}

		public Vec2i Set(Vec2d v) {
			this.x = (int)v.x;
			this.y = (int)v.y;
			return this;
		}

		public readonly int Get(int component) {
			return component switch {
				0 => this.x,
				1 => this.y,
				_ => throw new ArgumentException()
			};
		}

		public Vec2i SetComponent(int component, int value) {
			switch (component) {
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				default:
					throw new ArgumentException();
			}
			return this;
		}

		public readonly long lengthSqr => (long)this.x * this.x + (long)this.y * this.y;

		public static long LengthSqr(int x, int y) => (long)x * x + (long)y * y;

		public readonly double length => Math.Sqrt((long)this.x * this.x + (long)this.y * this.y);

		public static double Length(int x, int y) => Math.Sqrt((long)x * x + (long)y * y);

		public readonly double Distance<T>(T v) where T : IVec2i {
			int dx = this.x - v.X;
			int dy = this.y - v.Y;
			return Math.Sqrt((long)dx * dx + (long)dy * dy);
		}

		public readonly double Distance(int x, int y) {
			int dx = this.x - x;
			int dy = this.y - y;
			return Math.Sqrt((long)dx * dx + (long)dy * dy);
		}

		public readonly long SqrDistance<T>(T v) where T : IVec2i {
			int dx = this.x - v.X;
			int dy = this.y - v.Y;
			return (long)dx * dx + (long)dy * dy;
		}

		public readonly long SqrDistance(int x, int y) {
			int dx = this.x - x;
			int dy = this.y - y;
			return (long)dx * dx + (long)dy * dy;
		}

		public readonly long GridDistance<T>(T v) where T : IVec2i {
			return Math.Abs(v.X - this.x) + Math.Abs(v.Y - this.y);
		}

		public readonly long GridDistance(int x, int y) {
			return Math.Abs(x - this.x) + Math.Abs(y - this.y);
		}

		public static double Distance(int x1, int y1, int x2, int y2) {
			int dx = x1 - x2;
			int dy = y1 - y2;
			return Math.Sqrt((long)dx * dx + (long)dy * dy);
		}

		public static double SqrDistance(int x1, int y1, int x2, int y2) {
			int dx = x1 - x2;
			int dy = y1 - y2;
			return (long)dx * dx + (long)dy * dy;
		}

		public Vec2i Add<T>(T v) where T : IVec2i {
			return this.Add(v, ref this);
		}

		public readonly Vec2i Add<T>(T v, ref Vec2i dest) where T : IVec2i {
			dest.x = this.x + v.X;
			dest.y = this.y + v.Y;
			return dest;
		}

		public Vec2i Add(int x, int y) {
			return this.Add(x, y, ref this);
		}

		public readonly Vec2i Add(int x, int y, ref Vec2i destination) {
			destination.x = this.x + x;
			destination.y = this.y + y;
			return destination;
		}

		public Vec2i Subtract<T>(T v) where T : IVec2i {
			return this.Subtract(v, ref this);
		}

		public readonly Vec2i Subtract<T>(T v, ref Vec2i destination) where T : IVec2i {
			destination.x = this.x - v.X;
			destination.y = this.y - v.Y;
			return destination;
		}

		public Vec2i Subtract(int x, int y) {
			return this.Subtract(x, y, ref this);
		}

		public readonly Vec2i Subtract(int x, int y, ref Vec2i destination) {
			destination.x = this.x - x;
			destination.y = this.y - y;
			return destination;
		}

		public Vec2i Multiply(int scalar) {
			return this.Multiply(scalar, ref this);
		}

		public readonly Vec2i Multiply(int scalar, ref Vec2i destination) {
			destination.x = this.x * scalar;
			destination.y = this.y * scalar;
			return destination;
		}

		public Vec2i Multiply<T>(T v) where T : IVec2i {
			return this.Multiply(v, ref this);
		}

		public readonly Vec2i Multiply<T>(T v, ref Vec2i destination) where T : IVec2i {
			destination.x = this.x * v.X;
			destination.y = this.y * v.Y;
			return destination;
		}

		public Vec2i Multiply(int x, int y) {
			return this.Multiply(x, y, ref this);
		}

		public readonly Vec2i Multiply(int x, int y, ref Vec2i destination) {
			destination.x = this.x * x;
			destination.y = this.y * y;
			return destination;
		}

		public Vec2i Divide(float scalar) {
			return this.Divide(scalar, ref this);
		}

		public readonly Vec2i Divide(float scalar, ref Vec2i destination) {
			float invscalar = 1.0f / scalar;
			destination.x = (int)(this.x * invscalar);
			destination.y = (int)(this.y * invscalar);
			return destination;
		}

		public Vec2i Divide(int scalar) {
			return this.Divide(scalar, ref this);
		}

		public readonly Vec2i Divide(int scalar, ref Vec2i destination) {
			destination.x = this.x / scalar;
			destination.y = this.y / scalar;
			return destination;
		}

		public Vec2i Zero() {
			this.x = 0;
			this.y = 0;
			return this;
		}

		public readonly Vec2i negated => new(-this.x, -this.y);

		public Vec2i Negate() {
			return this.Negate(ref this);
		}

		public readonly Vec2i Negate(ref Vec2i destination) {
			destination.x = -this.x;
			destination.y = -this.y;
			return destination;
		}

		public Vec2i Min<T>(T v) where T : IVec2i {
			return this.Min(v, ref this);
		}

		public readonly Vec2i Min<T>(T v, ref Vec2i destination) where T : IVec2i {
			destination.x = this.x < v.X ? this.x : v.X;
			destination.y = this.y < v.Y ? this.y : v.Y;
			return destination;
		}

		public static Vec2i Min(Vec2i a, Vec2i b) {
			Vec2i rez = new();
			return a.Min(b, ref rez);
		}

		public Vec2i Max<T>(T v) where T : IVec2i {
			return this.Max(v, ref this);
		}

		public readonly Vec2i Max<T>(T v, ref Vec2i destination) where T : IVec2i {
			destination.x = this.x > v.X ? this.x : v.X;
			destination.y = this.y > v.Y ? this.y : v.Y;
			return destination;
		}

		public static Vec2i Max(Vec2i a, Vec2i b) {
			Vec2i rez = new();
			return a.Max(b, ref rez);
		}

		/// <summary> Returns the component index whose value is larger than the other </summary>
		public readonly int MaxComponent() {
			int absX = Math.Abs(this.x);
			int absY = Math.Abs(this.y);
			return absX >= absY ? 0 : 1;
		}

		/// <summary> Returns the component index whose value is smaller than the other </summary>
		public readonly int MinComponent() {
			int absX = Math.Abs(this.x);
			int absY = Math.Abs(this.y);
			return absX < absY ? 0 : 1;
		}

		public Vec2i Abs() {
			return this.Abs(ref this);
		}

		public readonly Vec2i Abs(ref Vec2i destination) {
			destination.x = Math.Abs(this.x);
			destination.y = Math.Abs(this.y);
			return destination;
		}

		public static Vec2i Abs(Vec2i v) {
			Vec2i rez = new();
			return v.Abs(ref rez);
		}

		public static Vec2i operator +(Vec2i a, Vec2i b) {
			Vec2i rez = new();
			return a.Add(b, ref rez);
		}

		public static Vec2i operator +(Vec2i a, int b) {
			Vec2i rez = new();
			return a.Add(b, b, ref rez);
		}

		public static Vec2i operator +(Vec2i a, (int x, int y) b) {
			Vec2i rez = new();
			return a.Add(b.x, b.y, ref rez);
		}

		public static Vec2i operator -(Vec2i a, Vec2i b) {
			Vec2i rez = new();
			return a.Subtract(b, ref rez);
		}

		public static Vec2i operator -(Vec2i a, int b) {
			Vec2i rez = new();
			return a.Subtract(b, b, ref rez);
		}

		public static Vec2i operator -(Vec2i a, (int x, int y) b) {
			Vec2i rez = new();
			return a.Subtract(b.x, b.y, ref rez);
		}

		public static Vec2i operator *(Vec2i a, Vec2i b) {
			Vec2i rez = new();
			return a.Multiply(b, ref rez);
		}

		public static Vec2i operator *(Vec2i a, int b) {
			Vec2i rez = new();
			return a.Multiply(b, ref rez);
		}

		public static Vec2i operator *(Vec2i a, (int x, int y) b) {
			Vec2i rez = new();
			return a.Multiply(b.x, b.y, ref rez);
		}

		public static Vec2i operator /(Vec2i a, float b) {
			Vec2i rez = new();
			return a.Divide(b, ref rez);
		}

		public static Vec2i operator /(Vec2i a, int b) {
			Vec2i rez = new();
			return a.Divide(b, ref rez);
		}

		public static bool operator ==(Vec2i a, Vec2i b) => a.Equals(b);
		public static bool operator !=(Vec2i a, Vec2i b) => !a.Equals(b);

		public readonly override int GetHashCode() {
			const int prime = 31;
			int result = 1;
			result = prime * result + this.x;
			result = prime * result + this.y;
			return result;
		}

		public readonly override bool Equals(object obj) {
			return obj is Vec2i other && this.Equals(other);
		}

		public readonly bool Equals(Vec2i other) {
			return this.x == other.x && this.y == other.y;
		}

		public readonly bool Equals(int x, int y) {
			return this.x == x && this.y == y;
		}

		public readonly override string ToString() {
			return $"({this.x},{this.y})";
		}
	}
}
