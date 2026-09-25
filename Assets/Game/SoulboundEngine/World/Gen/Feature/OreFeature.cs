namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Level;
	using System;

	public class OreFeature : Feature<OreFeature.Config> {
		public override bool Generate(FeaturePlaceContext<Config> context) {
			BlockPos origin = context.origin;
			IWorldGenLevel level = context.level;
			int size = context.config.size;
			BlockState oreState = context.config.oreState;
			BlockPos.Mutable pos = new();
			bool anyPlaced = false;
			
			for (int dx = 0; dx < size; dx++) {
				for (int dy = 0; dy < size; dy++) {
					if (this.CanPlaceOre(pos.Set(origin.x + dx, origin.y + dy), level.GetBlockState)) {
						anyPlaced = true;
						level.SetBlockState(origin.Offset(dx, dy), oreState);
					}
				}
			}
			return anyPlaced;
		}

		private bool CanPlaceOre(BlockPos blockPos, Func<BlockPos, BlockState> blockGetter) {
			return blockGetter(blockPos).block != Blocks.AIR;
		}

		public class Config : IFeatureConfig {
			public BlockState oreState { get; }
			public int size { get; }

			public Config(int size, BlockState oreState) {
				this.size = size;
				this.oreState = oreState;
			}
		}
	}
}
