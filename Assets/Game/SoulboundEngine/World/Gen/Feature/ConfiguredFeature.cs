namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Gen.Generator;
	using SoulboundEngine.World.Level;
	using System.Collections.Generic;

	public abstract record ConfiguredFeature {
		public static readonly RegistryKey<ConfiguredFeature> TREE = Create("tree");
		public static readonly RegistryKey<ConfiguredFeature> RUBY_ORE = Create("ore/ruby");

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public static ConfiguredFeature Init(RegistryBootstrapContext context, Registry<ConfiguredFeature> registry) {
			IBlockStateTest defaultOreReplaceables = IBlockStateTest.MatchBlock(Blocks.STONE);
			Registry<ConfiguredFeature>.Register(registry, RUBY_ORE, new ConfiguredFeature<OreFeature.Config, OreFeature>(
				(OreFeature)Feature.ORE,
				new OreFeature.Config(
					size: 2,
					density: 0.8d,
					new List<OreFeature.TargetBlockState> {
						OreFeature.Target(defaultOreReplaceables, Blocks.LEAVES.DefaultState)
					}
				)
			));
            return Registry<ConfiguredFeature>.Register(registry, TREE, new ConfiguredFeature<TreeFeature.Config, TreeFeature>(
                (TreeFeature)Feature.TREE,
                new TreeFeature.Config(
                    trunkState: Blocks.WOOD.DefaultState,
                    leafState: Blocks.LEAVES.DefaultState,
                    plantableOn: new HashSet<Block>() { Blocks.GRASS, Blocks.DIRT },
                    minTrunkHeight: 4,
                    trunkHeightVariance: 2,
                    canopyRadius: 2,
                    leafDensity: 0.8f
                )
            ));
		}

		private static RegistryKey<ConfiguredFeature> Create(string id) {
			return RegistryKey<ConfiguredFeature>.Of(RegistryKeys.CONFIGURED_FEATURE, Identifier.Of(id));
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
