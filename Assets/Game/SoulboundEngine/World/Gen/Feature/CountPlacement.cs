namespace SoulboundEngine.World.Gen.Feature {
    using SoulboundEngine.Common.Math.Random;
    using SoulboundEngine.World.Block;
    using System.Collections.Generic;

    public class CountPlacement : IPlacementModifier {
        private readonly int count;

        public CountPlacement(int count) {
            this.count = count;
        }

        public IEnumerable<BlockPos> GetPositions(PlacementContext context, IRandom random, BlockPos origin) {
            for (int i = 0; i < this.count; i++) {
                yield return origin;
            }
        }
    }
}
