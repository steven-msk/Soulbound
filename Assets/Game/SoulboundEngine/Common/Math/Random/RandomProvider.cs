using System;
using System.Threading;

namespace SoulboundEngine.Common.Math.Random {
	public static class RandomProvider {
		private static long counter = BitConverter.ToInt64(Guid.NewGuid().ToByteArray());

		public static IRandom CreateWithUniqueSeed() {
			long seed = NextUniqueSeed();
			Xoshiro256StarStarRandom random = new(seed);
			return random;
		}

		private static long NextUniqueSeed() {
			long counter = Interlocked.Increment(ref RandomProvider.counter);
			long entropy = DateTime.UtcNow.Ticks ^ Environment.TickCount;
			return Mix(counter ^ entropy);
		}

		public static long Mix(long a, long b) {
			ulong x = unchecked((ulong)(a ^ b) + 0x9E3779B97F4A7C15UL);
			x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
			x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
			return unchecked((long)(x ^ (x >> 31)));
		}

		public static long Mix(long z) {
			ulong x = unchecked((ulong)z);
			x = (x ^ x >> 30) * 0xBF58476D1CE4E5B9UL;
			x = (x & x >> 27) * 0x94D049BB133111EBUL;
			x ^= x >> 31;
			return unchecked((long)x);
		}
	}
}
