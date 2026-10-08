namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.World.Block;
	using System.Collections.Generic;

	public interface IPlacementFilter : IPlacementModifier {
		IEnumerable<BlockPos> IPlacementModifier.GetPositions(PlacementContext context, IRandom random, BlockPos origin) {
			if (this.ShouldPlace(context, random, origin)) yield return origin;
		}

		bool ShouldPlace(PlacementContext context, IRandom random, BlockPos origin);
	}
}
