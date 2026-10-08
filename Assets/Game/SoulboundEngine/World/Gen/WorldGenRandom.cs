namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common.Math.Random;

	public class WorldGenRandom : IRandom {
		private readonly IRandom random;

		public WorldGenRandom(IRandom random) {
			this.random = random;
		}

		public IPositionalRandomFactory ForkPositional() => this.random.ForkPositional();

		public IRandom NewInstance(long seed) => this.random.NewInstance(seed);

		public long SetDecorationSeed(long seed, int chunkX) {
			this.SetSeed(seed);
			long scale = this.NextLong() | 1L;
			long result = chunkX * scale ^ seed;
			this.SetSeed(result);
			return result;
		}

		public void SetFeatureSeed(long seed, int index, int step) {
			this.SetSeed(seed + index + step * 10000);
		}

		public void SetLargeFeatureSeed(long seed, int chunkX) {
			this.SetSeed(seed);
			long scale = this.NextLong();
			this.SetSeed(chunkX * scale ^ seed);
		}

		public bool NextBool() {
			return this.random.NextBool();
		}

		public double NextDouble() {
			return this.random.NextDouble();
		}

		public float NextFloat() {
			return this.random.NextFloat();
		}

		public double NextGaussian() {
			return this.random.NextGaussian();
		}

		public int NextInt() {
			return this.random.NextInt();
		}

		public long NextLong() {
			return this.random.NextLong();
		}

		public void SetSeed(long seed) => this.random.SetSeed(seed);
	}
}
