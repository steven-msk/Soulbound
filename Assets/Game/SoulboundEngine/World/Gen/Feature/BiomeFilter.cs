namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Gen.Biome;
	using System;

	public class BiomeFilter : IPlacementFilter {
		private static readonly BiomeFilter INSTANCE = new();
		public static BiomeFilter FromGenSettings => INSTANCE;

		private BiomeFilter() {
		}

		public bool ShouldPlace(PlacementContext context, IRandom random, BlockPos origin) {
			PlacedFeature feature = context.topFeature 
				?? throw new InvalidOperationException("Tried to biome filter an unregistered feature, or a feature that should not restrict the biome");
			RegistryEntry<Biome> biome = context.level.GetBiome(origin);
			return context.generator.GetBiomeGenSettings(biome).HasFeature(feature);
		}
	}
}
