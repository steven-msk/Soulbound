namespace SoulboundEngine.Common.Math {
	public static class Maths {
		public static int FloorDiv(int a, int b) {
			int q = a / b;
			if ((a % b != 0) && ((a < 0) != (b < 0))) q--;
			return q;
		}
	}
}
