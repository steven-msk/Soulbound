namespace SoulboundEngine.Common.Math.Random {
	public sealed class Xoshiro256StarStarRandom : IRandom {
		private ulong s0, s1, s2, s3;

		public Xoshiro256StarStarRandom(long seed) => this.SetSeed(seed);

		public void SetSeed(long seed) {
			// Use SplitMix64 to expand a single seed into well-distributed state
			ulong sm = (ulong)seed;
			this.s0 = SplitMix64(ref sm);
			this.s1 = SplitMix64(ref sm);
			this.s2 = SplitMix64(ref sm);
			this.s3 = SplitMix64(ref sm);
		}

		private static ulong SplitMix64(ref ulong state) {
			state += 0x9E3779B97F4A7C15UL;
			ulong z = state;
			z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
			z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
			return z ^ (z >> 31);
		}

		private static ulong RotL(ulong x, int k) => (x << k) | (x >> (64 - k));

		private ulong NextUInt64() {
			ulong result = RotL(this.s1 * 5, 7) * 9;
			ulong t = this.s1 << 17;

			this.s2 ^= this.s0;
			this.s3 ^= this.s1;
			this.s1 ^= this.s2;
			this.s0 ^= this.s3;
			this.s2 ^= t;
			this.s3 = RotL(this.s3, 45);

			return result;
		}

		public long NextLong() => unchecked((long)this.NextUInt64());
		public int NextInt() => unchecked((int)(this.NextUInt64() >> 32));
		public bool NextBool() => (this.NextUInt64() & 1) == 1;

		public double NextDouble() =>
			(this.NextUInt64() >> 11) * (1.0 / (1UL << 53)); // 53-bit precision, [0,1)

		public float NextFloat() =>
			(this.NextUInt64() >> 40) * (1.0f / (1 << 24)); // 24-bit precision, [0,1)

		public double NextGaussian() {
			// Box-Muller, no caching for simplicity; add caching later if profiling says so
			double u1 = 1.0 - this.NextDouble();
			double u2 = this.NextDouble();
			return System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
				   System.Math.Cos(2.0 * System.Math.PI * u2);
		}
	}
}
