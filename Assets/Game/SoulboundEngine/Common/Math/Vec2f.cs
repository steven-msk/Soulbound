using System;

namespace SoulboundEngine.Common.Math {
	using Math = System.Math;

	public struct Vec2f : IVec2f, IEquatable<Vec2f> {
		public static readonly Vec2f ZERO = new(0.0f, 0.0f);
		public static readonly Vec2f ONE = new(1.0f, 1.0f);
		public static readonly Vec2f UNIT_X = new(1.0f, 0.0f);
		public static readonly Vec2f UNIT_Y = new(0.0f, 1.0f);
		public static readonly Vec2f MAX = new(float.MaxValue, float.MaxValue);
		public static readonly Vec2f MIN = new(float.MinValue, float.MinValue);
		public float x;
		public float y;

		public Vec2f(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public Vec2f(float d) {
			this.x = d;
			this.y = d;
		}

		readonly float IVec2f.X => this.x;
		readonly float IVec2f.Y => this.y;

		public readonly Vec2i ToVec2i() => new((int)this.x, (int)this.y);

		public readonly Vec2d ToVec2d() => new(this.x, this.y);

		public Vec2f Set(float d) {
			this.x = d;
			this.y = d;
			return this;
		}

		public Vec2f Set(float x, float y) {
			this.x = x;
			this.y = y;
			return this;
		}

		public Vec2f Set(double d) {
			this.x = (float)d;
			this.y = (float)d;
			return this;
		}

		public Vec2f Set(double x, double y) {
			this.x = (float)x;
			this.y = (float)y;
			return this;
		}

		public Vec2f Set<T>(T v) where T : IVec2f {
			this.x = v.X;
			this.y = v.Y;
			return this;
		}

		public Vec2f Set(Vec2d v) {
			this.x = (float)v.x;
			this.y = (float)v.y;
			return this;
		}

		public Vec2f Set(Vec2i v) {
			this.x = v.x;
			this.y = v.y;
			return this;
		}

		public readonly float Get(int component) {
			return component switch {
				0 => this.x,
				1 => this.y,
				_ => throw new ArgumentException()
			};
		}

		public Vec2f SetComponent(int component, float value) {
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

		public Vec2f Perpendicular() {
			float temp = this.y;
			this.y = this.x * -1;
			this.x = temp;
			return this;
		}

		public static Vec2f Perpendicular(Vec2f v) {
			return v.Perpendicular();
		}

		public readonly Vec2f Scale(float s) {
			return new Vec2f(this.x * s, this.y * s);
		}

		public readonly float Dot(Vec2f v) {
			return this.x * v.x + this.y * v.y;
		}

		public static float Dot(Vec2f a, Vec2f b) {
			return a.Dot(b);
		}

		public readonly float Angle<T>(T v) where T : IVec2f {
			float dot = this.x * v.X + this.y * v.Y;
			float det = this.x * v.Y - this.y * v.X;
			return (float)Math.Atan2(det, dot);
		}

		public static float Angle(Vec2f a, Vec2f b) {
			return a.Angle(b);
		}

		public Vec2f Zero() {
			this.x = 0;
			this.y = 0;
			return this;
		}

		public readonly Vec2f negated => new(-this.x, -this.y);

		public Vec2f Negate() {
			return this.Negate(ref this);
		}

		public readonly Vec2f Negate(ref Vec2f destination) {
			destination.x = -this.x;
			destination.y = -this.y;
			return destination;
		}

		public Vec2f Add<T>(T v) where T : IVec2f {
			return this.Add(v, ref this);
		}

		public readonly Vec2f Add<T>(T v, ref Vec2f dest) where T : IVec2f {
			dest.x = this.x + v.X;
			dest.y = this.y + v.Y;
			return dest;
		}

		public Vec2f Add(float x, float y) {
			return this.Add(x, y, ref this);
		}

		public readonly Vec2f Add(float x, float y, ref Vec2f destination) {
			destination.x = this.x + x;
			destination.y = this.y + y;
			return destination;
		}

		public Vec2f Subtract<T>(T v) where T : IVec2f {
			return this.Subtract(v, ref this);
		}

		public readonly Vec2f Subtract<T>(T v, ref Vec2f destination) where T : IVec2f {
			destination.x = this.x - v.X;
			destination.y = this.y - v.Y;
			return destination;
		}

		public Vec2f Subtract(float x, float y) {
			return this.Subtract(x, y, ref this);
		}

		public readonly Vec2f Subtract(float x, float y, ref Vec2f destination) {
			destination.x = this.x - x;
			destination.y = this.y - y;
			return destination;
		}

		public Vec2f Multiply(float scalar) {
			return this.Multiply(scalar, ref this);
		}

		public readonly Vec2f Multiply(float scalar, ref Vec2f destination) {
			destination.x = this.x * scalar;
			destination.y = this.y * scalar;
			return destination;
		}

		public Vec2f Multiply(float x, float y) {
			return this.Multiply(x, y, ref this);
		}

		public readonly Vec2f Multiply(float x, float y, ref Vec2f destination) {
			destination.x = this.x * x;
			destination.y = this.y * y;
			return destination;
		}

		public Vec2f Multiply<T>(T v) where T : IVec2f {
			return this.Multiply(v, ref this);
		}

		public readonly Vec2f Multiply<T>(T v, ref Vec2f destination) where T : IVec2f {
			destination.x = this.x * v.X;
			destination.y = this.y * v.Y;
			return destination;
		}

		public Vec2f Divide(float scalar) {
			return this.Divide(scalar, ref this);
		}

		public readonly Vec2f Divide(float scalar, ref Vec2f destination) {
			float inv = 1.0f / scalar;
			destination.x = this.x * inv;
			destination.y = this.y * inv;
			return destination;
		}

		public Vec2f Divide<T>(T v) where T : IVec2f {
			return this.Divide(v, ref this);
		}

		public readonly Vec2f Divide<T>(T v, ref Vec2f destination) where T : IVec2f {
			destination.x = this.x / v.X;
			destination.y = this.y / v.Y;
			return destination;
		}

		public Vec2f Divide(float x, float y) {
			return this.Divide(x, y, ref this);
		}

		public readonly Vec2f Divide(float x, float y, ref Vec2f destination) {
			destination.x = this.x / x;
			destination.y = this.y / y;
			return destination;
		}

		public Vec2f Lerp<T>(T other, float t) where T : IVec2f {
			return this.Lerp(other, t, ref this);
		}

		public readonly Vec2f Lerp<T>(T other, float t, ref Vec2f destination) where T : IVec2f {
			destination.x = this.x + (other.X - this.x) * t;
			destination.y = this.y + (other.Y - this.y) * t;
			return destination;
		}

		public static Vec2f Lerp(Vec2f from, Vec2f to, float t) {
			Vec2f rez = new();
			return from.Lerp(to, t, ref rez);
		}

		public readonly Vec2f normalized { 
			get {
				float dist = (float)Math.Sqrt(this.x * this.x + this.y * this.y);
				return dist < 1.0e-4f ? ZERO : new Vec2f(this.x / dist, this.y / dist);
			}
		}

		public readonly float length => (float)Math.Sqrt(this.x * this.x + this.y * this.y);

		public readonly float lengthSqr => this.x * this.x + this.y * this.y;

		public static float Length(float x, float y) {
			return (float)Math.Sqrt(x * x + y * y);
		}

		public readonly float SqrDistance<T>(T p) where T : IVec2f {
			float xd = p.X - this.x;
			float yd = p.Y - this.y;
			return xd * xd + yd * yd;
		}

		public readonly float Distance(float x, float y) {
			float dx = this.x - x;
			float dy = this.y - y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		public static float Distance(float x1, float y1, float x2, float y2) {
			float dx = x1 - x2;
			float dy = y1 - y2;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		public static float SqrDistance(float x1, float y1, float x2, float y2) {
			float dx = x1 - x2;
			float dy = y1 - y2;
			return dx * dx + dy * dy;
		}

		public Vec2f Normalize() {
			return this.Normalize(ref this);
		}

		public readonly Vec2f Normalize(ref Vec2f destination) {
			float invLength = 1.0f / (float)Math.Sqrt(this.x * this.x + this.y * this.y);
			destination.x = this.x * invLength;
			destination.y = this.y * invLength;
			return destination;
		}

		public Vec2f Normalize(float length) {
			return this.Normalize(length, ref this);
		}

		public readonly Vec2f Normalize(float length, ref Vec2f destination) {
			float invLength = length / (float)Math.Sqrt(this.x * this.x + this.y * this.y);
			destination.x = this.x * invLength;
			destination.y = this.y * invLength;
			return destination;
		}

		public static Vec2f Normalize(Vec2f v, float length = 1.0f) {
			Vec2f rez = new();
			return v.Normalize(length, ref rez);
		}

		public Vec2f Min<T>(T other) where T : IVec2f {
			return this.Min(other, ref this);
		}

		public readonly Vec2f Min<T>(T other, ref Vec2f destination) where T : IVec2f {
			destination.x = this.x < other.X ? this.x : other.X;
			destination.y = this.y < other.Y ? this.y : other.Y;
			return destination;
		}

		public static Vec2f Min(Vec2f a, Vec2f b) {
			Vec2f rez = new();
			return a.Min(b, ref rez);
		}

		public Vec2f Max<T>(T other) where T : IVec2f {
			return this.Max(other, ref this);
		}

		public readonly Vec2f Max<T>(T other, ref Vec2f destination) where T : IVec2f {
			destination.x = this.x > other.X ? this.x : other.X;
			destination.y = this.y > other.Y ? this.y : other.Y;
			return destination;
		}

		public static Vec2f Max(Vec2f a, Vec2f b) {
			Vec2f rez = new();
			return a.Max(b, ref rez);
		}

		public readonly int MaxComponent() {
			float absX = Math.Abs(this.x);
			float absY = Math.Abs(this.y);
			return absX >= absY ? 0 : 1;
		}

		public readonly int MinComponent() {
			float absX = Math.Abs(this.x);
			float absY = Math.Abs(this.y);
			return absX < absY ? 0 : 1;
		}

		public Vec2f Floor() {
			return this.Floor(ref this);
		}

		public readonly Vec2f Floor(ref Vec2f destination) {
			destination.x = (float)Math.Floor(this.x);
			destination.y = (float)Math.Floor(this.y);
			return destination;
		}

		public static Vec2f Floor(Vec2f v) {
			Vec2f rez = new();
			return v.Floor(ref rez);
		}

		public Vec2f Ceil() {
			return this.Ceil(ref this);
		}

		public readonly Vec2f Ceil(ref Vec2f destination) {
			destination.x = (float)Math.Ceiling(this.x);
			destination.y = (float)Math.Ceiling(this.y);
			return destination;
		}

		public static Vec2f Ceil(Vec2f v) {
			Vec2f rez = new();
			return v.Ceil(ref rez);
		}

		public Vec2f Round() {
			return this.Round(ref this);
		}

		public readonly Vec2f Round(ref Vec2f destination) {
			destination.x = (float)Math.Round(this.x);
			destination.y = (float)Math.Round(this.y);
			return destination;
		}

		public static Vec2f Round(Vec2f v) {
			Vec2f rez = new();
			return v.Round(ref rez);
		}

		public readonly bool IsFinite => float.IsFinite(this.x) && float.IsFinite(this.y);

		public Vec2f Abs() {
			return this.Abs(ref this);
		}

		public readonly Vec2f Abs(ref Vec2f destination) {
			destination.x = Math.Abs(this.x);
			destination.y = Math.Abs(this.y);
			return destination;
		}

		public static Vec2f Abs(Vec2f v) {
			Vec2f rez = new();
			return v.Abs(ref rez);
		}

		public static Vec2f operator +(Vec2f a, Vec2f b) {
			Vec2f rez = new();
			return a.Add(b, ref rez);
		}

		public static Vec2f operator +(Vec2f a, float b) {
			Vec2f rez = new();
			return a.Add(b, b, ref rez);
		}

		public static Vec2f operator +(Vec2f a, (float x, float y) b) {
			Vec2f rez = new();
			return a.Add(b.x, b.y, ref rez);
		}

		public static Vec2f operator -(Vec2f a, Vec2f b) {
			Vec2f rez = new();
			return a.Subtract(b, ref rez);
		}

		public static Vec2f operator -(Vec2f a, float b) {
			Vec2f rez = new();
			return a.Subtract(b, b, ref rez);
		}

		public static Vec2f operator -(Vec2f a, (float x, float y) b) {
			Vec2f rez = new();
			return a.Subtract(b.x, b.y, ref rez);
		}

		public static Vec2f operator *(Vec2f a, Vec2f b) {
			Vec2f rez = new();
			return a.Multiply(b, ref rez);
		}

		public static Vec2f operator *(Vec2f a, float b) {
			Vec2f rez = new();
			return a.Multiply(b, ref rez);
		}

		public static Vec2f operator *(Vec2f a, (float x, float y) b) {
			Vec2f rez = new();
			return a.Multiply(b.x, b.y, ref rez);
		}

		public static Vec2f operator /(Vec2f a, Vec2f b) {
			Vec2f rez = new();
			return a.Divide(b, ref rez);
		}

		public static Vec2f operator /(Vec2f a, float b) {
			Vec2f rez = new();
			return a.Divide(b, ref rez);
		}

		public static Vec2f operator /(Vec2f a, (float x, float y) b) {
			Vec2f rez = new();
			return a.Divide(b.x, b.y, ref rez);
		}

		public readonly bool Equals(Vec2f other) {
			return this.Equals(this.x, other.x) && this.Equals(this.y, other.y);
		}

		public readonly bool Equals(Vec2f other, float delta) {
			return Equals(this.x, other.x, delta) && Equals(this.y, other.y, delta);
		}

		private static bool Equals(float a, float b, float delta) {
			return BitConverter.SingleToInt32Bits(a) == BitConverter.SingleToInt32Bits(b) || Math.Abs(a - b) <= delta;
		}

		public readonly bool Equals(float x, float y) {
			return BitConverter.SingleToInt32Bits(this.x) == BitConverter.SingleToInt32Bits(x)
				&& BitConverter.SingleToInt32Bits(this.y) == BitConverter.SingleToInt32Bits(y);
		}

		public readonly override int GetHashCode() {
			const int prime = 31;
			int result = 1;
			result = prime * result + BitConverter.SingleToInt32Bits(this.x);
			result = prime * result + BitConverter.SingleToInt32Bits(this.y);
			return result;
		}

		public readonly override bool Equals(object obj) {
			return obj is Vec2f other && this.Equals(other);
		}

		public readonly override string ToString() {
			return $"({this.x},{this.y});";
		}
	}
}
