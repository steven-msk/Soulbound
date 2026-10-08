namespace SoulboundEngine.World.Gen.Feature {
    using SoulboundEngine.Common.Math.Random;
    using SoulboundEngine.World.Block;
    using SoulboundEngine.World.Chunk;
    using System.Collections.Generic;

    public class InLinePlacement : IPlacementModifier {
        private static readonly InLinePlacement INSTANCE = new();
		public static InLinePlacement Spread => INSTANCE;

        private InLinePlacement() {
        }

        public IEnumerable<BlockPos> GetPositions(PlacementContext context, IRandom random, BlockPos origin) {
            int x = origin.x + random.NextInt(0, ChunkSection.WIDTH);
            yield return new BlockPos(x, origin.y);
        }
    }
}
