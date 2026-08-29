namespace SoulboundEngine.Common.Math.Random {
	using SoulboundEngine.Registry;
	using System.Collections.Generic;

	public class RandomSequences {
		private readonly long worldSeed;
		private long salt;
		private readonly Dictionary<Identifier, IRandom> sequences = new();

		public RandomSequences(long worldSeed, long salt = 0L) {
			this.worldSeed = worldSeed;
			this.salt = salt;
		}

		public IRandom GetOrCreate(Identifier id) {
			if (this.sequences.TryGetValue(id, out IRandom existing)) {
				return existing;
			}

			long seed = this.DeriveSeed(id);
			IRandom random = new Xoshiro256StarStarRandom(seed);
			this.sequences[id] = random;
			return random;
		}

		// Resets every sequence without changing the world seed itself.
		public void ResetSalt(long newSalt) {
			this.salt = newSalt;
			this.sequences.Clear();
		}

		private long DeriveSeed(Identifier id) {
			long idHash = StableHash(id.ToString());
			return Mix(this.worldSeed, Mix(this.salt, idHash));
		}

		private static long Mix(long a, long b) {
			ulong x = unchecked((ulong)(a ^ b) + 0x9E3779B97F4A7C15UL);
			x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
			x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
			return unchecked((long)(x ^ (x >> 31)));
		}

		// FNV-1a
		private static long StableHash(string s) {
			ulong hash = 14695981039346656037UL;
			foreach (char c in s) {
				hash ^= c;
				hash *= 1099511628211UL;
			}
			return unchecked((long)hash);
		}
	}
}
