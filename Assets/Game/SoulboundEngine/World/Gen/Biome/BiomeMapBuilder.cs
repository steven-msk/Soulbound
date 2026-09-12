namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using System;

	public sealed class BiomeMapBuilder {
		public void AddBiomes(Action<RegistryKey<Biome>, Climate.ParameterPoint> addBiome) {
			addBiome(Biome.PLAINS, Climate.ParameterPoint.Create(b => b
				.Span(Climate.ParameterType.SHAPE, -2f, 0f)
			));
			addBiome(Biome.HILLS, Climate.ParameterPoint.Create(b => b
				.Span(Climate.ParameterType.SHAPE, 0f, 2f)
			));
		}
	}
}
