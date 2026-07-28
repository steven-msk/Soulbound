namespace SoulboundEngine.Common.Math.Random {
	public interface IRandom {
		bool NextBool();
		double NextDouble();
		float NextFloat();
		double NextGaussian();
		int NextInt();
		long NextLong();

		public int NextInt(int minInclusive, int maxExclusive) {
			double t = this.NextDouble();
			return (int)(minInclusive + (maxExclusive - minInclusive) * t);
		}

		void SetSeed(long seed);
	}
}
