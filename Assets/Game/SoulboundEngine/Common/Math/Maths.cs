namespace SoulboundEngine.Common.Math {
	using Math = System.Math;

	public static class Maths {
		public const double RAD_2_DEG = 0.01745329251994329576923690768489d;
		public const double DEG_2_RAD = 57.295779513082320876798154814105d;

		public static int FloorDiv(int a, int b) {
			int q = a / b;
			if ((a % b != 0) && ((a < 0) != (b < 0))) q--;
			return q;
		}

		public static double LengthSqr(double x, double y) {
			return x * x + y * y;
		}

		public static double Lerp(double a, double b, double t) {
			return a + (b - a) * t;
		}

		public static int FloorToInt(double x) {
			return (int)Math.Floor(x);
		}

		public static bool AreEqual(double a, double b, double delta = 1.0E-5d) {
			return Math.Abs(b - a) < delta;
		}

		public static double SmoothStep(double from, double to, double t) {
			t = Clamp01(t);
			t = -2.0d * t * t * t + 3.0d * t * t;
			return to * t + from * (1.0d - t);
		}

		public static double Clamp01(double v) {
			return Math.Clamp(v, 0.0d, 1.0d);
		}

		public static float Clamp01(float v) {
			return Math.Clamp(v, 0f, 1f);
		}

		public static double InverseLerp(double a, double b, double value) {
			return a != b ? Clamp01((value - a) / (b - a)) : 0.0d;
		}

		public static double Clamp(double value, double min, double max) {
			return Math.Clamp(value, min, max);
		}

		public static float Clamp(float value, float min, float max) {
			return Math.Clamp(value, min, max);
		}
	}
}
