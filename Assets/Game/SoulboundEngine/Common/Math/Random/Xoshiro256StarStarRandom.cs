namespace SoulboundEngine.Common.Math.Random {
	public sealed class Xoshiro256StarStarRandom : IRandom {
		private ulong s0, s1, s2, s3;

		public Xoshiro256StarStarRandom(long seed) => this.SetSeed(seed);

		private Xoshiro256StarStarRandom(ulong s0, ulong s1, ulong s2, ulong s3) {
			this.s0 = s0; this.s1 = s1; this.s2 = s2; this.s3 = s3;
			if ((s0 | s1 | s2 | s3) == 0) this.SetSeed(0);
		}

		public void SetSeed(long seed) {
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

		public IPositionalRandomFactory ForkPositional() => new PositionalFactory(this.NextLong());

		public IRandom NewInstance(long seed) => new Xoshiro256StarStarRandom(seed);

		public sealed class PositionalFactory : IPositionalRandomFactory {
			private readonly ulong s0, s1, s2, s3;

			public PositionalFactory(long seed) {
				ulong sm = (ulong)seed;
				this.s0 = SplitMix64(ref sm);
				this.s1 = SplitMix64(ref sm);
				this.s2 = SplitMix64(ref sm);
				this.s3 = SplitMix64(ref sm);
			}

			public IRandom At(int x, int y, int z) {
				ulong hash = (ulong)Maths.PositionHash(x, y, z);
				return new Xoshiro256StarStarRandom(
					this.s0 ^ hash,
					this.s1 ^ RotL(hash, 32),
					this.s2,
					this.s3
				);
			}

			public IRandom FromHashOf(string s) {
				(ulong lo, ulong hi) = StableHash128(s);
				return new Xoshiro256StarStarRandom(
					this.s0 ^ lo,
					this.s1 ^ hi,
					this.s2,
					this.s3
				);
			}

			public IRandom FromSeed(long seed) {
				ulong u = (ulong)seed;
				return new Xoshiro256StarStarRandom(
					this.s0 ^ u,
					this.s1,
					this.s2,
					this.s3
				);
			}

			private static (ulong, ulong) StableHash128(string s) {
				ulong h1 = 14695981039346656037UL;
				ulong h2 = 1099511628211UL;
				foreach (char c in s) {
					h1 = (h1 ^ c) * 1099511628211UL;
					h2 = (h2 ^ c) * 14695981039346656037UL;
				}
				return (h1, h2);
			}
		}
	}
}
