namespace SoulboundEngine.Common.Math {
	public readonly struct Axis {
		private readonly bool isHorizontal;

		public static readonly Axis X = new(true, Direction.Right);
		public static readonly Axis Y = new(false, Direction.Up);

		private Axis(bool isHorizontal, Direction positive) {
			this.isHorizontal = isHorizontal;
			this.positive = positive;
			this.negative = positive.Opposite();
		}

		public readonly Direction positive { get; }
		public readonly Direction negative { get; }

		public int Get(int x, int y) => this.isHorizontal ? x : y;

		public bool Get(bool x, bool y) => this.isHorizontal ? x : y;

		public double Get(double x, double y) => this.isHorizontal ? x : y;
	}
}
