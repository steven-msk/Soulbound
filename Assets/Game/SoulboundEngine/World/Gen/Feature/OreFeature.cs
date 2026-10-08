namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Level;
	using System;
	using System.Collections.Generic;

	public class OreFeature : Feature<OreFeature.Config> {
		public override bool Generate(FeaturePlaceContext<Config> context) {
			BlockPos origin = context.origin;
			IRandom random = context.random;
			IWorldGenLevel level = context.level;
			List<TargetBlockState> targets = context.config.targets;
			int size = context.config.size;
			double frequency = context.config.density;
			BlockPos.Mutable pos = new();
			bool anyPlaced = false;
			
			for (int dx = 0; dx < size; dx++) {
				for (int dy = 0; dy < size; dy++) {
					foreach (TargetBlockState target in targets) {
						if (!this.CanPlaceOre(random, pos.Set(origin.x + dx, origin.y + dy), level.GetBlockState, target)) {
							continue;
						}

						if (random.NextDouble() < frequency) {
							anyPlaced = true;
							level.SetBlockState(pos, target.replace);
							break;
						}
					}
				}
			}
			return anyPlaced;
		}

		private bool CanPlaceOre(IRandom random, BlockPos blockPos, Func<BlockPos, BlockState> blockGetter, TargetBlockState targetBlockState) {
			BlockState blockState = blockGetter(blockPos);
			return targetBlockState.targetRule.Test(blockState, random);
		}

		public static TargetBlockState Target(IBlockStateTest targetRule, BlockState replace) {
			return new TargetBlockState(targetRule, replace);
		}

		public class Config : IFeatureConfig {
			public List<TargetBlockState> targets { get; }
			public int size { get; }
			public double density { get; }

			public Config(int size, double density, List<TargetBlockState> targets) {
				this.targets = targets;
				this.size = size;
				this.density = density;
			}
		}

		public sealed record TargetBlockState(IBlockStateTest targetRule, BlockState replace);
	}
}
