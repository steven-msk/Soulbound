namespace SoulboundEngine.World.Gen.Feature {
    using SoulboundEngine.Common.Math.Random;
    using SoulboundEngine.World.Block;
    using SoulboundEngine.World.Block.State;
    using System;
    using System.Collections.Generic;

    public class BlockFilter : IPlacementModifier {
        private readonly Predicate<BlockState> predicate;
        private readonly int yOffset;

        public BlockFilter(Predicate<BlockState> predicate, int yOffset = -1) {
            this.predicate = predicate;
            this.yOffset = yOffset;
        }

        public IEnumerable<BlockPos> GetPositions(PlacementContext context, IRandom random, BlockPos origin) {
            BlockState state = context.GetBlockState(origin.Offset(0, this.yOffset));
            if (this.predicate(state)) yield return origin;
        }
    }
}
