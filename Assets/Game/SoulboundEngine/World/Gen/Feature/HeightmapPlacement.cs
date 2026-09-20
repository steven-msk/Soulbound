namespace SoulboundEngine.World.Gen.Feature {
    using SoulboundEngine.Common.Math.Random;
    using SoulboundEngine.World.Block;
    using System.Collections.Generic;

    public class HeightmapPlacement : IPlacementModifier {
        public static readonly HeightmapPlacement WORLD_SURFACE = new();

        private HeightmapPlacement() {
        }

        public IEnumerable<BlockPos> GetPositions(PlacementContext context, IRandom random, BlockPos origin) {
            int height = context.GetHeight(origin.x);
            yield return new BlockPos(origin.x, height);
        }
    }
}
