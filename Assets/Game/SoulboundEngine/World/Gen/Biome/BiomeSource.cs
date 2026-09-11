namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using System;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public abstract class BiomeSource : IBiomeResolver {
		private HashSet<RegistryEntry<Biome>>? possibleBiomes;
		private readonly Func<HashSet<RegistryEntry<Biome>>> possibleBiomesSupplier;

		protected BiomeSource() {
			this.possibleBiomesSupplier = () => this.possibleBiomes ??= this.CollectPossibleBiomes().ToHashSet();
		}

		protected abstract IEnumerable<RegistryEntry<Biome>> CollectPossibleBiomes();

		public HashSet<RegistryEntry<Biome>> GetPossibleBiomes() => this.possibleBiomesSupplier();

		public abstract RegistryEntry<Biome> GetNoiseBiome(int x, int y, Climate.Sampler sampler);
	}
}
