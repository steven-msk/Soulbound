namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Gen.Generator;
	using SoulboundEngine.World.Level;
	using System;

	public abstract class Feature {
		public static readonly Feature<NoOpFeature.Config> NO_OP = Register("no_op", new NoOpFeature());
        public static readonly Feature<TreeFeature.Config> TREE = Register("tree", new TreeFeature());

		private static Feature<C> Register<C>(string id, Feature<C> feature) where C : IFeatureConfig {
			return Registry<Feature>.Register(Registries.FEATURE, RegistryKey<Feature>.Of(RegistryKeys.FEATURE, Identifier.Of(id)), feature);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
		public static Feature Init(RegistryBootstrapContext context, Registry<Feature> registry) => TREE;

		public static bool IsAdjacentToAir(Func<BlockPos, BlockState> blockGetter, BlockPos blockPos) {
			return TestAdjacentStates(blockGetter, blockPos, s => s.IsAir());
		}

		public static bool TestAdjacentStates(Func<BlockPos, BlockState> blockGetter, BlockPos blockPos, Predicate<BlockState> predicate) {
			foreach (BlockPos neighborPos in blockPos.GetCardinalNeighbors()) {
				if (predicate(blockGetter(neighborPos))) return true;
			}
			return false;
		}

		public abstract bool Generate(IFeatureConfig config, IWorldGenLevel level, ChunkGenerator chunkGenerator, IRandom random, BlockPos origin);
	}

	public abstract class Feature<C> : Feature where C : IFeatureConfig {

		public abstract bool Generate(FeaturePlaceContext<C> context);

		public sealed override bool Generate(IFeatureConfig config, IWorldGenLevel level, ChunkGenerator chunkGenerator, IRandom random, BlockPos origin) {
			return level.IsValidForSetBlock(origin) && this.Generate(new FeaturePlaceContext<C>(level, chunkGenerator, random, origin, (C)config));
		}
	}
}
