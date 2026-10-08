namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using System;

	public sealed class BiomeMapBuilder {
		private static readonly Climate.Parameter HALF_RANGE_NEGATIVE = Climate.Parameter.Span(-1f, 0f);
		private static readonly Climate.Parameter HALF_RANGE_POSITIVE = Climate.Parameter.Span(0f, 1f);

		public void AddBiomes(Action<RegistryKey<Biome>, Climate.ParameterPoint> addBiome) {
			addBiome(Biome.PLAINS, Climate.ParameterPoint.Create(b => b
				.Add(Climate.ParameterType.SHAPE, HALF_RANGE_NEGATIVE)
			));
			addBiome(Biome.HILLS, Climate.ParameterPoint.Create(b => b
				.Add(Climate.ParameterType.SHAPE, HALF_RANGE_POSITIVE)
			));
		}
	}
}
