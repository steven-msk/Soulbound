namespace SoulboundEngine.Client.World.BlockSystem.States {
	public static class StateHasher {
		private const uint FnvOffset = 2166136261;
		private const uint FnvPrime = 16777619;

		public static int ComputeHash(Block block, BlockPropertyEntries properties) {
			uint hash = FnvOffset;
			hash = HashString(hash, block.GetIdentifier().ToString());

			foreach (var (property, value) in properties.GetSorted()) {
				hash = HashString(hash, property);

				string stringValue = CanonicalValue(value);
				hash = HashString(hash, stringValue);
			}

			return unchecked((int)hash);
		}

		private static string CanonicalValue(object value) {
			return value switch {
				float f => f.ToString("R"),
				double d => d.ToString("R"),
				_ => value.ToString()!
			};
		}

		private static uint HashInt(uint hash, int value) {
			unchecked {
				hash ^= (uint)value;
				hash *= FnvPrime;
				return hash;
			}
		}

		private static uint HashString(uint hash, string value) {
			unchecked {
				foreach (char c in value) {
					hash ^= c;
					hash *= FnvPrime;
				}
				return hash;
			}
		}
	}
}
