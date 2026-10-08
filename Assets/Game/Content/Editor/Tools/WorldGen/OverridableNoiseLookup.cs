namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Gen.Noise;
	using System.Collections.Generic;

#nullable enable

	public sealed class OverridableNoiseLookup : IRegistryEntryLookup<NormalNoise.Parameters> {
		private readonly IRegistryEntryOwner<NormalNoise.Parameters> owner;
		private readonly IRegistryEntryLookup<NormalNoise.Parameters> baseLookup;
		private readonly Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides = new();

		public OverridableNoiseLookup(
			IRegistryEntryOwner<NormalNoise.Parameters> owner,
			IRegistryEntryLookup<NormalNoise.Parameters> baseLookup,
			Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides
		) {
			this.owner = owner;
			this.baseLookup = baseLookup;
			this.overrides = overrides;
		}

		public RegistryEntry<NormalNoise.Parameters>? Get(RegistryKey<NormalNoise.Parameters> key) {
			return this.overrides.TryGetValue(key, out NormalNoise.Parameters overriden)
				? new RegistryEntry<NormalNoise.Parameters>(this.owner, key, overriden)
				: this.baseLookup.Get(key);
		}

		public IEnumerable<RegistryKey<NormalNoise.Parameters>> GetAllKeys() {
			return this.baseLookup.GetAllKeys();
		}

		public RegistryEntry<NormalNoise.Parameters> GetOrThrow(RegistryKey<NormalNoise.Parameters> key) {
			return this.overrides.TryGetValue(key, out NormalNoise.Parameters overriden)
				? new RegistryEntry<NormalNoise.Parameters>(this.owner, key, overriden)
				: this.baseLookup.GetOrThrow(key);
		}
	}
}
