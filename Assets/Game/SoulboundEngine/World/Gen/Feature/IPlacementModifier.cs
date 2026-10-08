namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.World.Block;
	using System.Collections.Generic;

	public interface IPlacementModifier {
		IEnumerable<BlockPos> GetPositions(PlacementContext context, IRandom random, BlockPos origin);
	}
}
