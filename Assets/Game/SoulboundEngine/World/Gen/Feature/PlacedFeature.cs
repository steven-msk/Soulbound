namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Collection;
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Gen.Generator;
	using SoulboundEngine.World.Level;
	using System.Collections.Generic;
	using System.Linq;

	public record PlacedFeature(RegistryEntry<ConfiguredFeature> feature, List<IPlacementModifier> placement) {
		public static readonly RegistryKey<PlacedFeature> TREE = Create("tree");

		public static PlacedFeature Init(RegistryBootstrapContext context, Registry<PlacedFeature> registry) {
			Registry<ConfiguredFeature> configured = context.Lookup(RegistryKeys.CONFIGURED_FEATURE);
			return Registry<PlacedFeature>.Register(registry, TREE, new PlacedFeature(
                configured.GetOrThrow(ConfiguredFeature.TREE),
                new List<IPlacementModifier> {
                    new CountPlacement(4),
                    InLinePlacement.Spread,
                    HeightmapPlacement.WorldSurface,
					BiomeFilter.FromGenSettings,
                    new BlockFilter(state => state.GetBlock() == Blocks.GRASS || state.GetBlock() == Blocks.DIRT)
                }
			));
		}

		private static RegistryKey<PlacedFeature> Create(string id) {
			return RegistryKey<PlacedFeature>.Of(RegistryKeys.PLACED_FEATURE, Identifier.Of(id));
		}

		public bool GenerateUnregistered(IWorldGenLevel level, ChunkGenerator chunkGenerator, IRandom random, BlockPos origin) {
			return this.Generate(new PlacementContext(level, chunkGenerator, null), random, origin);
		}

		public bool Generate(IWorldGenLevel level, ChunkGenerator chunkGenerator, IRandom random, BlockPos origin) {
			return this.Generate(new PlacementContext(level, chunkGenerator, this), random, origin);
		}

		private bool Generate(PlacementContext context, IRandom random, BlockPos origin) {
			IEnumerable<BlockPos> placements = new List<BlockPos>() { origin };
			foreach (IPlacementModifier placementModifier in this.placement) {
				placements = placements.SelectMany(p => placementModifier.GetPositions(context, random, p)).ToList();
			}

			ConfiguredFeature feature = this.feature.GetValue();
			bool placedAny = false;
			placements.ForEach(pos => {
				if (feature.Generate(context.level, context.generator, random, pos)) {
					placedAny = true;
				}
			});
			return placedAny;
		}

		// TODO: risk of recursion in GetDecoratedFeatures() getter
		public IEnumerable<RegistryEntry<ConfiguredFeature>> GetDecoratedFeatures() {
			return Enumerable.Concat(Collections.Single(this.feature), this.feature.GetValue().GetSubFeatures());
		}
	}
}
