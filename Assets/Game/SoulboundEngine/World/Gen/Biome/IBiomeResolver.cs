namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using System;

	public interface IBiomeResolver {
		RegistryEntry<Biome> GetNoiseBiome(int x, int y, Climate.Sampler sampler);

		public static IBiomeResolver Of(Func<int, int, Climate.Sampler, RegistryEntry<Biome>> func) {
			return new DelegateImpl(func);
		}

		private sealed record DelegateImpl(Func<int, int, Climate.Sampler, RegistryEntry<Biome>> func) : IBiomeResolver {
			public RegistryEntry<Biome> GetNoiseBiome(int x, int y, Climate.Sampler sampler) {
				return this.func(x, y, sampler);
			}
		}
	}
}
