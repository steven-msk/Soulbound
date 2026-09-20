namespace SoulboundEngine.World.Gen.Feature {
    using SoulboundEngine.Common.Math.Random;
    using SoulboundEngine.World.Block;
    using SoulboundEngine.World.Block.State;
    using SoulboundEngine.World.Level;
    using System.Collections.Generic;

    public class TreeFeature : Feature<TreeFeature.Config> {
		public override bool Generate(FeaturePlaceContext<Config> context) {
            Config config = context.config;
            IWorldGenLevel level = context.level;
            IRandom random = context.random;
            BlockPos origin = context.origin;

            if (!CanGenerateAt(level, origin, config)) return false;

            int trunkHeight = config.minTrunkHeight + random.NextInt(0, config.trunkHeightVariance + 1);
            if (!HasRoom(level, origin, trunkHeight, config.canopyRadius)) return false;

            PlaceTrunk(level, origin, trunkHeight, config.trunkState);
            PlaceCanopy(level, origin, trunkHeight, random, config);
            return true;
		}

        private static bool CanGenerateAt(IWorldGenLevel level, BlockPos origin, Config config) {
            BlockPos below = origin.Down();
            return config.plantableOn.Contains(level.GetBlockState(below).GetBlock()) && level.GetBlockState(origin).IsAir();
        }

        private static bool HasRoom(IWorldGenLevel level, BlockPos origin, int trunkHeight, int canopyRadius) {
            for (int y = 0; y < trunkHeight + canopyRadius; y++) {
                if (!level.GetBlockState(origin.Up(y)).IsAir()) return false;
            }
            return true;
        }

        private static void PlaceTrunk(IWorldGenLevel level, BlockPos origin, int trunkHeight, BlockState trunkState) {
            for (int y = 0; y < trunkHeight; y++) {
                level.SetBlockState(origin.Up(y), trunkState);
            }
        }

        private static void PlaceCanopy(IWorldGenLevel level, BlockPos origin, int trunkHeight, IRandom random, Config config) {
            BlockPos center = origin.Up(trunkHeight);
            int r = config.canopyRadius;
            for (int dx = -r; dx <= r; dx++) {
                for (int dy = -r; dy <= r; dy++) {
                    if (dx * dx + dy * dy > r * r) continue;
                    BlockPos leafPos = center.Offset(dx, dy);
                    if (level.GetBlockState(leafPos).IsAir() && random.NextFloat() < config.leafDensity) {
                        level.SetBlockState(leafPos, config.leafState);
                    }
                }
            }
        }

        public class Config : IFeatureConfig {
            public readonly BlockState trunkState;
            public readonly BlockState leafState;
            public readonly HashSet<Block> plantableOn;
            public readonly int minTrunkHeight;
            public readonly int trunkHeightVariance;
            public readonly int canopyRadius;
            public readonly float leafDensity;

            public Config(BlockState trunkState, BlockState leafState, HashSet<Block> plantableOn,
                int minTrunkHeight, int trunkHeightVariance, int canopyRadius, float leafDensity) {
                this.trunkState = trunkState;
                this.leafState = leafState;
                this.plantableOn = plantableOn;
                this.minTrunkHeight = minTrunkHeight;
                this.trunkHeightVariance = trunkHeightVariance;
                this.canopyRadius = canopyRadius;
                this.leafDensity = leafDensity;
            }
        }
    }
}
