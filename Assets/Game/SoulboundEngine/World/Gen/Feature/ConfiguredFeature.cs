namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Gen.Generator;
	using SoulboundEngine.World.Level;
	using System.Collections.Generic;

	public abstract record ConfiguredFeature {
		public static ConfiguredFeature Init(RegistryBootstrapContext context, Registry<ConfiguredFeature> registry) {
			return null;
		}

		public abstract IFeatureConfig GetConfig();

		public abstract Feature GetFeature();

		public abstract bool Generate(IWorldGenLevel level, ChunkGenerator chunkGenerator, IRandom random, BlockPos origin);

		public IEnumerable<RegistryEntry<ConfiguredFeature>> GetSubFeatures() => this.GetConfig().GetSubFeatures();
	}

	public record ConfiguredFeature<FC, F>(F feature, FC config) : ConfiguredFeature where F : Feature<FC> where FC : IFeatureConfig {
		public override bool Generate(IWorldGenLevel level, ChunkGenerator chunkGenerator, IRandom random, BlockPos origin) {
			return this.feature.Generate(this.config, level, chunkGenerator, random, origin);
		}

		public sealed override IFeatureConfig GetConfig() => this.config;

		public sealed override Feature GetFeature() => this.feature;
	}
}
