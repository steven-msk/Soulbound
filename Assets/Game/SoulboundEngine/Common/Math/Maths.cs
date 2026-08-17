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
	}
}
