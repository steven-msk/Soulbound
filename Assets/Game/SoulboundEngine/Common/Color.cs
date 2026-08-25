namespace SoulboundEngine.Common {
	using SoulboundEngine.Common.Math;
	using System;

#nullable enable

	public readonly struct Color : IEquatable<Color> {
		public const float GOLDEN_RATIO_CONJUGATE = 0.618033988749895f;
		public static readonly Color WHITE = new(1f, 1f, 1f);
		public static readonly Color BLACK = new(0f, 0f, 0f);
		public static readonly Color CLEAR = new(0f, 0f, 0f, 0f);
		public static readonly Color GRAY = new(0.5f, 0.5f, 0.5f);
		public static readonly Color RED = new(1f, 0f, 0f);
		public static readonly Color GREEN = new(0f, 1f, 0f);
		public static readonly Color BLUE = new(0f, 0f, 1f);
		public static readonly Color YELLOW = new(1f, 1f, 0f);
		public static readonly Color CYAN = new(0f, 1f, 1f);
		public static readonly Color MAGENTA = new(1f, 0f, 1f);
		public static readonly Color ORANGE = new(1f, 0.647f, 0f);
		public static readonly Color PURPLE = new(0.5f, 0f, 0.5f);
		public static readonly Color PINK = new(1f, 0.753f, 0.796f);
		public static readonly Color BROWN = new(0.647f, 0.165f, 0.165f);

		public readonly float r;
		public readonly float g;
		public readonly float b;
		public readonly float a;

		public Color(float r, float g, float b, float a = 1f) {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public byte R8 => (byte)Maths.Clamp(MathF.Round(this.r * 255f), 0f, 255f);
		public byte G8 => (byte)Maths.Clamp(MathF.Round(this.g * 255f), 0f, 255f);
		public byte B8 => (byte)Maths.Clamp(MathF.Round(this.b * 255f), 0f, 255f);
		public byte A8 => (byte)Maths.Clamp(MathF.Round(this.a * 255f), 0f, 255f);

		public static Color FromBytes(byte r, byte g, byte b, byte a = 255) {
			return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
		}

		public uint ToRGBA32() {
			return ((uint)this.R8 << 24) | ((uint)this.G8 << 16) | ((uint)this.B8 << 8) | this.A8;
		}

		public static Color FromRGBA32(uint packed) {
			return FromBytes(
				(byte)((packed >> 24) & 0xFF),
				(byte)((packed >> 16) & 0xFF),
				(byte)((packed >> 8) & 0xFF),
				(byte)(packed & 0xFF)
			);
		}

		public uint ToARGB32() {
			return ((uint)this.A8 << 24) | ((uint)this.R8 << 16) | ((uint)this.G8 << 8) | this.B8;
		}

		public static Color FromARGB32(uint packed) {
			return FromBytes(
				(byte)((packed >> 16) & 0xFF),
				(byte)((packed >> 8) & 0xFF),
				(byte)(packed & 0xFF),
				(byte)((packed >> 24) & 0xFF)
			);
		}

		public static Color FromHex(string hex) {
			string h = hex.TrimStart('#');
			return h.Length switch {
				6 => FromBytes(
					Convert.ToByte(h[..2], 16),
					Convert.ToByte(h.Substring(2, 2), 16),
					Convert.ToByte(h.Substring(4, 2), 16)
				),
				8 => FromBytes(
					Convert.ToByte(h[..2], 16),
					Convert.ToByte(h.Substring(2, 2), 16),
					Convert.ToByte(h.Substring(4, 2), 16),
					Convert.ToByte(h.Substring(6, 2), 16)
				),
				_ => throw new FormatException($"Invalid hex color string: '{hex}'")
			};
		}

		public string ToHex(bool includeAlpha = false) {
			return includeAlpha
				? $"#{this.R8:X2}{this.G8:X2}{this.B8:X2}{this.A8:X2}"
				: $"#{this.R8:X2}{this.G8:X2}{this.B8:X2}";
		}

		public static Color FromHSV(float h, float s, float v, float a = 1f) {
			h = ((h % 360f) + 360f) % 360f; // normalize to [0, 360)
			s = Maths.Clamp(s, 0f, 1f);
			v = Maths.Clamp(v, 0f, 1f);

			float c = v * s;
			float x = c * (1f - MathF.Abs((h / 60f) % 2f - 1f));
			float m = v - c;

			(float r1, float g1, float b1) = h switch {
				< 60f => (c, x, 0f),
				< 120f => (x, c, 0f),
				< 180f => (0f, c, x),
				< 240f => (0f, x, c),
				< 300f => (x, 0f, c),
				_ => (c, 0f, x)
			};

			return new Color(r1 + m, g1 + m, b1 + m, a);
		}

		public void ToHSV(out float h, out float s, out float v) {
			float max = MathF.Max(this.r, MathF.Max(this.g, this.b));
			float min = MathF.Min(this.r, MathF.Min(this.g, this.b));
			float delta = max - min;

			v = max;
			s = max <= 0f ? 0f : delta / max;

			h = delta <= 0f
				? 0f
				: max == this.r
					? 60f * (((this.g - this.b) / delta) % 6f)
					: max == this.g ? 60f * (((this.b - this.r) / delta) + 2f) : 60f * (((this.r - this.g) / delta) + 4f);

			if (h < 0f) h += 360f;
		}

		public static Color FromHSL(float h, float s, float l, float a = 1f) {
			h = ((h % 360f) + 360f) % 360f;
			s = Maths.Clamp(s, 0f, 1f);
			l = Maths.Clamp(l, 0f, 1f);

			float c = (1f - MathF.Abs(2f * l - 1f)) * s;
			float x = c * (1f - MathF.Abs((h / 60f) % 2f - 1f));
			float m = l - c / 2f;

			(float r1, float g1, float b1) = h switch {
				< 60f => (c, x, 0f),
				< 120f => (x, c, 0f),
				< 180f => (0f, c, x),
				< 240f => (0f, x, c),
				< 300f => (x, 0f, c),
				_ => (c, 0f, x)
			};

			return new Color(r1 + m, g1 + m, b1 + m, a);
		}

		public void ToHSL(out float h, out float s, out float l) {
			float max = MathF.Max(this.r, MathF.Max(this.g, this.b));
			float min = MathF.Min(this.r, MathF.Min(this.g, this.b));
			float delta = max - min;

			l = (max + min) / 2f;
			s = delta <= 0f ? 0f : delta / (1f - MathF.Abs(2f * l - 1f));

			h = delta <= 0f
				? 0f
				: max == this.r
					? 60f * (((this.g - this.b) / delta) % 6f)
					: max == this.g ? 60f * (((this.b - this.r) / delta) + 2f) : 60f * (((this.r - this.g) / delta) + 4f);

			if (h < 0f) h += 360f;
		}

		public float Grayscale() => (this.r + this.g + this.b) / 3f;

		public float Luminance() => 0.2126f * this.r + 0.7152f * this.g + 0.0722f * this.b;

		public bool IsPerceivedDark(float threshold = 0.5f) => this.Luminance() < threshold;

		public Color WithR(float r) => new(r, this.g, this.b, this.a);
		public Color WithG(float g) => new(this.r, g, this.b, this.a);
		public Color WithB(float b) => new(this.r, this.g, b, this.a);
		public Color WithAlpha(float a) => new(this.r, this.g, this.b, a);

		public Color Clamped() => new(
			Maths.Clamp(this.r, 0f, 1f),
			Maths.Clamp(this.g, 0f, 1f),
			Maths.Clamp(this.b, 0f, 1f),
			Maths.Clamp(this.a, 0f, 1f)
		);

		public Color Invert() => new(1f - this.r, 1f - this.g, 1f - this.b, this.a);

		public Color Multiply(Color other) => new(
			this.r * other.r, this.g * other.g, this.b * other.b, this.a * other.a
		);

		public Color AlphaBlendOver(Color dst) {
			float outA = this.a + dst.a * (1f - this.a);
			if (outA <= 0f) return new Color(0f, 0f, 0f, 0f);
			float r1 = (this.r * this.a + dst.r * dst.a * (1f - this.a)) / outA;
			float g1 = (this.g * this.a + dst.g * dst.a * (1f - this.a)) / outA;
			float b1 = (this.b * this.a + dst.b * dst.a * (1f - this.a)) / outA;
			return new Color(r1, g1, b1, outA);
		}

		public static Color Lerp(Color a, Color b, float t) {
			t = Maths.Clamp(t, 0f, 1f);
			return LerpUnclamped(a, b, t);
		}

		public static Color LerpUnclamped(Color a, Color b, float t) {
			return new Color(
				a.r + (b.r - a.r) * t,
				a.g + (b.g - a.g) * t,
				a.b + (b.b - a.b) * t,
				a.a + (b.a - a.a) * t
			);
		}

		public static uint Fnv1aHash(string input) {
			unchecked {
				const uint fnvOffsetBasis = 2166136261;
				const uint fnvPrime = 16777619;
				uint hash = fnvOffsetBasis;
				foreach (char c in input) {
					hash ^= c;
					hash *= fnvPrime;
				}
				return hash;
			}
		}

		public static Color operator +(Color a, Color b) => new(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
		public static Color operator -(Color a, Color b) => new(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
		public static Color operator *(Color a, float scalar) => new(a.r * scalar, a.g * scalar, a.b * scalar, a.a * scalar);
		public static Color operator *(float scalar, Color a) => a * scalar;
		public static Color operator *(Color a, Color b) => a.Multiply(b);
		public static Color operator /(Color a, float scalar) => new(a.r / scalar, a.g / scalar, a.b / scalar, a.a / scalar);
		public static bool operator ==(Color a, Color b) => a.Equals(b);
		public static bool operator !=(Color a, Color b) => !a.Equals(b);

		public bool Equals(Color other) {
			return this.r.Equals(other.r) && this.g.Equals(other.g) && this.b.Equals(other.b) && this.a.Equals(other.a);
		}

		public override bool Equals(object? obj) => obj is Color other && this.Equals(other);
		public override int GetHashCode() => HashCode.Combine(this.r, this.g, this.b, this.a);
		public override string ToString() => $"RGBA({this.r:F3}, {this.g:F3}, {this.b:F3}, {this.a:F3})";
		public void Deconstruct(out float r, out float g, out float b, out float a) => (r, g, b, a) = (this.r, this.g, this.b, this.a);
	}
}
