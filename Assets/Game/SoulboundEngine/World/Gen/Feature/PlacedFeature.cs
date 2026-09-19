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
		public static PlacedFeature Init(RegistryBootstrapContext context, Registry<PlacedFeature> registry) {
			return null;
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
