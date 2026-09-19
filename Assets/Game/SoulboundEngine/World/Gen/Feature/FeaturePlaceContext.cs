namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Level;

#nullable enable

	public record FeaturePlaceContext<FC>(
		IWorldGenLevel level,
		ChunkGenerator chunkGenerator,
		IRandom random,
		BlockPos origin,
		FC config
	) where FC : IFeatureConfig {
	}
}
