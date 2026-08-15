namespace SoulboundEngine.Common.Math {
	public static class Maths {
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
	}
}
