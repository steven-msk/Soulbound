using System;

namespace SoulboundEngine.Common.Math {
	using Math = System.Math;

	public struct Vec2d : IVec2d, IEquatable<Vec2d> {
		public static readonly Vec2d ZERO = new(0.0d, 0.0d);
		public static readonly Vec2d ONE = new(1.0d, 1.0d);
		public static readonly Vec2d UNIT_X = new(1.0d, 0.0d);
		public static readonly Vec2d UNIT_Y = new(0.0d, 1.0d);
		public static readonly Vec2d MAX = new(double.MaxValue, double.MaxValue);
		public static readonly Vec2d MIN = new(double.MinValue, double.MinValue);
		public double x;
		public double y;

		public Vec2d(double x, double y) {
			this.x = x;
			this.y = y;
		}

		public Vec2d(double d) {
			this.x = d;
			this.y = d;
		}

		readonly double IVec2d.X => this.x;
		readonly double IVec2d.Y => this.y;

		public readonly Vec2f ToVec2f() => new((float)this.x, (float)this.y);

		public readonly Vec2i ToVec2i() => new((int)this.x, (int)this.y);

		public Vec2d Set(double d) {
			this.x = d;
			this.y = d;
			return this;
		}

		public Vec2d Set(double x, double y) {
			this.x = x;
			this.y = y;
			return this;
		}

		public Vec2d Set<T>(T v) where T : IVec2d {
			this.x = v.X;
			this.y = v.Y;
			return this;
		}

		public Vec2d Set(Vec2f v) {
			this.x = v.x;
			this.y = v.y;
			return this;
		}

		public Vec2d Set(Vec2i v) {
			this.x = v.x;
			this.y = v.y;
			return this;
		}

		public readonly Vec2i Round(MidpointRounding midpointRounding = MidpointRounding.AwayFromZero) {
			Vec2i dest = new();
			return this.RoundTo(ref dest, midpointRounding);
		}

		public readonly Vec2i RoundTo(ref Vec2i destination, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero) {
			destination.x = (int)Math.Round(this.x, midpointRounding);
			destination.y = (int)Math.Round(this.y, midpointRounding);
			return destination;
		}
		
		public Vec2d Round(int digits, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero) {
			return this.Round(ref this, digits, midpointRounding);
		}

		public readonly Vec2d Round(ref Vec2d destination, int digits, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero) {
			destination.x = Math.Round(this.x, digits, midpointRounding);
			destination.y = Math.Round(this.y, digits, midpointRounding);
			return destination;
		}

		public static Vec2d Round(Vec2d v, int digits, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero) {
			Vec2d rez = new();
			return v.Round(ref rez, digits, midpointRounding);
		}

		public readonly double Get(int component) {
			return component switch {
				0 => this.x,
				1 => this.y,
				_ => throw new ArgumentException()
			}; ;
		}

		public Vec2d SetComponent(int component, double value) {
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

		public Vec2d Perpendicular() {
			double temp = this.y;
			this.y = this.x * -1;
			this.x = temp;
			return this;
		}

		public static Vec2d Perpendicular(Vec2d v) {
			return v.Perpendicular();
		}

		public Vec2d Add<T>(T v) where T : IVec2d {
			return this.Add(v, ref this);
		}

		public readonly Vec2d Add<T>(T v, ref Vec2d destination) where T : IVec2d {
			destination.x = this.x + v.X;
			destination.y = this.y + v.Y;
			return destination;
		}

		public static Vec2d operator +(Vec2d a, Vec2d b) {
			Vec2d rez = new();
			return a.Add(b, ref rez);
		}

		public Vec2d Add(double x, double y) {
			return this.Add(x, y, ref this);
		}

		public readonly Vec2d Add(double x, double y, ref Vec2d destination) {
			destination.x = this.x + x;
			destination.y = this.y + y;
			return destination;
		}

		public static Vec2d operator +(Vec2d a, (double x, double y) b) {
			Vec2d rez = new();
			return a.Add(b.x, b.y, ref rez);
		}

		public Vec2d Subtract<T>(T v) where T : IVec2d {
			return this.Subtract(v, ref this);
		}

		public readonly Vec2d Subtract<T>(T v, ref Vec2d destination) where T : IVec2d {
			destination.x = this.x - v.X;
			destination.y = this.y - v.Y;
			return destination;
		}

		public static Vec2d operator -(Vec2d a, Vec2d b) {
			Vec2d rez = new();
			return a.Subtract(b, ref rez);
		}

		public Vec2d Subtract(double x, double y) {
			return this.Subtract(x, y, ref this);
		}

		public readonly Vec2d Subtract(double x, double y, ref Vec2d destination) {
			destination.x = this.x - x;
			destination.y = this.y - y;
			return destination;
		}

		public static Vec2d operator -(Vec2d a, (double x, double y) b) {
			Vec2d rez = new();
			return a.Subtract(b.x, b.y, ref rez);
		}

		public Vec2d Multiply(double scalar) {
			return this.Multiply(scalar, ref this);
		}

		public readonly Vec2d Multiply(double scalar, ref Vec2d destination) {
			destination.x = this.x * scalar;
			destination.y = this.y * scalar;
			return destination;
		}

		public static Vec2d operator *(Vec2d a, double scalar) {
			Vec2d rez = new();
			return a.Multiply(scalar, ref rez);
		}

		public Vec2d Multiply(double x, double y) {
			return this.Multiply(x, y, ref this);
		}

		public readonly Vec2d Multiply(double x, double y, ref Vec2d destination) {
			destination.x = this.x * x;
			destination.y = this.y * y;
			return destination;
		}

		public static Vec2d operator *(Vec2d a, (double x, double y) b) {
			Vec2d rez = new();
			return a.Multiply(b.x, b.y, ref rez);
		}

		public Vec2d Multiply<T>(T v) where T : IVec2d {
			return this.Multiply(v, ref this);
		}

		public readonly Vec2d Multiply<T>(T v, ref Vec2d destination) where T : IVec2d {
			destination.x = this.x * v.X;
			destination.y = this.y * v.Y;
			return destination;
		}
		public static Vec2d operator *(Vec2d a, Vec2d b) {
			Vec2d rez = new();
			return a.Multiply(b, ref rez);
		}

		public Vec2d Divide(double scalar) {
			return this.Divide(scalar, ref this);
		}

		public readonly Vec2d Divide(double scalar, ref Vec2d destination) {
			double inv = 1.0d / scalar;
			destination.x = this.x * inv;
			destination.y = this.y * inv;
			return destination;
		}

		public static Vec2d operator /(Vec2d a, double scalar) {
			Vec2d rez = new();
			return a.Divide(scalar, ref rez);
		}

		public Vec2d Divide(double x, double y) {
			return this.Divide(x, y, ref this);
		}

		public readonly Vec2d Divide(double x, double y, ref Vec2d destination) {
			destination.x = this.x / x;
			destination.y = this.y / y;
			return destination;
		}

		public static Vec2d operator /(Vec2d a, (double x, double y) b) {
			Vec2d rez = new();
			return a.Divide(b.x, b.y, ref rez);
		}

		public Vec2d Divide<T>(T v) where T : IVec2d {
			return this.Divide(v, ref this);
		}

		public readonly Vec2d Divide<T>(T v, ref Vec2d destination) where T : IVec2d {
			destination.x = this.x / v.X;
			destination.y = this.y / v.Y;
			return destination;
		}

		public static Vec2d operator /(Vec2d a, Vec2d b) {
			Vec2d rez = new();
			return a.Divide(b, ref rez);
		}

		public readonly double Dot<T>(T v) where T : IVec2d {
			return this.x * v.X + this.y * v.Y;
		}

		public static double Dot(Vec2d a, Vec2d b) {
			return a.Dot(b);
		}

		public readonly double Angle<T>(T v) where T : IVec2d {
			double dot = this.x * v.X + this.y * v.Y;
			double det = this.x * v.Y - this.y * v.X;
			return Math.Atan2(det, dot);
		}

		public static double Angle(Vec2d a, Vec2d b) {
			return a.Angle(b);
		}

		public readonly double lengthSqr => this.x * this.x + this.y * this.y;

		public static double LengthSqr(double x, double y) => x * x + y * y;

		public readonly double length => Math.Sqrt(this.x * this.x + this.y * this.y);

		public static double Length(double x, double y) => Math.Sqrt(x * x + y * y);

		public readonly double Distance<T>(T v) where T : IVec2d {
			double dx = this.x - v.X;
			double dy = this.y - v.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		public readonly double SqrDistance<T>(T v) where T : IVec2d {
			double dx = this.x - v.X;
			double dy = this.y - v.Y;
			return dx * dx + dy * dy;
		}

		public readonly double Distance(double x, double y) {
			double dx = this.x - x;
			double dy = this.y - y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		public readonly double SqrDistance(double x, double y) {
			double dx = this.x - x;
			double dy = this.y - y;
			return dx * dx + dy * dy;
		}

		public static double Distance(double x1, double y1, double x2, double y2) {
			double dx = x1 - x2;
			double dy = y1 - y2;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		public static double SqrDistance(double x1, double y1, double x2, double y2) {
			double dx = x1 - x2;
			double dy = y1 - y2;
			return dx * dx + dy * dy;
		}

		public Vec2d Normalize() {
			return this.Normalize(ref this);
		}

		public readonly Vec2d Normalize(ref Vec2d destination) {
			double invLength = 1.0d / Math.Sqrt(this.x * this.x + this.y * this.y);
			destination.x = this.x * invLength;
			destination.y = this.y * invLength;
			return destination;
		}

		public Vec2d Normalize(double length) {
			return this.Normalize(length, ref this);
		}

		public readonly Vec2d Normalize(double length, ref Vec2d destination) {
			double invLength = length / Math.Sqrt(this.x * this.x + this.y * this.y);
			destination.x = this.x * invLength;
			destination.y = this.y * invLength;
			return destination;
		}

		public static Vec2d Normalize(Vec2d v, double length = 1.0d) {
			Vec2d rez = new();
			return v.Normalize(length, ref rez);
		}

		public Vec2d Zero() {
			this.x = 0;
			this.y = 0;
			return this;
		}

		public readonly Vec2d negated => new(-this.x, -this.y);

		public Vec2d Negate() {
			return this.Negate(ref this);
		}

		public readonly Vec2d Negate(ref Vec2d destination) {
			destination.x = -this.x;
			destination.y = -this.y;
			return destination;
		}

		public Vec2d Lerp<T>(T v, double t) where T : IVec2d {
			return this.Lerp(v, t, ref this);
		}

		public readonly Vec2d Lerp<T>(T v, double t, ref Vec2d destination) where T : IVec2d {
			destination.x = this.x + (v.X - this.x) * t;
			destination.y = this.y + (v.Y - this.y) * t;
			return destination;
		}

		public static Vec2d Lerp(Vec2d from, Vec2d to, double t) {
			Vec2d rez = new();
			return from.Lerp(to, t, ref rez);
		}

		public Vec2d Min<T>(T v) where T : IVec2d {
			return this.Min(v, ref this);
		}

		public readonly Vec2d Min<T>(T v, ref Vec2d destination) where T : IVec2d {
			destination.x = this.x < v.X ? this.x : v.X;
			destination.y = this.y < v.Y ? this.y : v.Y;
			return destination;
		}

		public static Vec2d Min(Vec2d a, Vec2d b) {
			Vec2d rez = new();
			return a.Min(b, ref rez);
		}

		public Vec2d Max<T>(T v) where T : IVec2d {
			return this.Max(v, ref this);
		}

		public readonly Vec2d Max<T>(T v, ref Vec2d destination) where T : IVec2d {
			destination.x = this.x > v.X ? this.x : v.X;
			destination.y = this.y > v.Y ? this.y : v.Y;
			return destination;
		}

		public static Vec2d Max(Vec2d a, Vec2d b) {
			Vec2d rez = new();
			return a.Max(b, ref rez);
		}

		public readonly int MaxComponent() {
			double absX = Math.Abs(this.x);
			double absY = Math.Abs(this.y);
			return absX >= absY ? 0 : 1;
		}

		public readonly int MinComponent() {
			double absX = Math.Abs(this.x);
			double absY = Math.Abs(this.y);
			return absX < absY ? 0 : 1;
		}

		public Vec2d Floor() {
			return this.Floor(ref this);
		}

		public readonly Vec2d Floor(ref Vec2d destination) {
			destination.x = Math.Floor(this.x);
			destination.y = Math.Floor(this.y);
			return destination;
		}

		public static Vec2d Floor(Vec2d v) {
			Vec2d rez = new();
			return v.Floor(ref rez);
		}

		public Vec2d Ceil() {
			return this.Ceil(ref this);
		}

		public readonly Vec2d Ceil(ref Vec2d destination) {
			destination.x = Math.Ceiling(this.x);
			destination.y = Math.Ceiling(this.y);
			return destination;
		}

		public static Vec2d Ceil(Vec2d v) {
			Vec2d rez = new();
			return v.Ceil(ref rez);
		}

		public readonly bool IsFinite => double.IsFinite(this.x) && double.IsFinite(this.y);

		public Vec2d Abs() {
			return this.Abs(ref this);
		}

		public readonly Vec2d Abs(ref Vec2d destination) {
			destination.x = Math.Abs(this.x);
			destination.y = Math.Abs(this.y);
			return destination;
		}

		public static Vec2d Abs(Vec2d v) {
			Vec2d rez = new();
			return v.Abs(ref rez);
		}

		public readonly override int GetHashCode() {
			const int prime = 31;
			int result = 1;
			long temp;
			temp = BitConverter.DoubleToInt64Bits(this.x);
			result = prime * result + (int)(temp ^ (temp >> 32));
			temp = BitConverter.DoubleToInt64Bits(this.y);
			result = prime * result + (int)(temp ^ (temp >> 32));
			return result;
		}

		public readonly override bool Equals(object obj) {
			return obj is Vec2d other && this.Equals(other);
		}

		public readonly bool Equals(Vec2d other) {
			return AreEqual(this.x, other.x) && AreEqual(this.y, other.y);
		}

		public readonly bool Equals(Vec2d other, double delta) {
			return Equals(this.x, other.x, delta) && Equals(this.y, other.y, delta);
		}

		public readonly bool Equals(double x, double y) {
			return AreEqual(this.x, x) && AreEqual(this.y, y);
		}

		private static bool Equals(double a, double b, double delta) {
			return BitConverter.DoubleToInt64Bits(a) == BitConverter.DoubleToInt64Bits(b) || Math.Abs(a - b) <= delta;
		}

		private static bool AreEqual(double a, double b) {
			return BitConverter.DoubleToInt64Bits(a) == BitConverter.DoubleToInt64Bits(b);
		}

		public readonly override string ToString() {
			return $"({this.x},{this.y})";
		}
	}
}
