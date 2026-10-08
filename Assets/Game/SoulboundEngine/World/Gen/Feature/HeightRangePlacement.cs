namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Gen.HeightProvider;
	using System.Collections.Generic;

	public class HeightRangePlacement : IPlacementModifier {
		private readonly IHeightProvider heightProvider;

		private HeightRangePlacement(IHeightProvider heightProvider) {
			this.heightProvider = heightProvider;
		}

		public static HeightRangePlacement Of(IHeightProvider heightProvider) {
			return new HeightRangePlacement(heightProvider);
		}

		public static HeightRangePlacement Uniform(IVerticalAnchor minInclusive, IVerticalAnchor maxInclusive) {
			return new HeightRangePlacement(UniformHeight.Of(minInclusive, maxInclusive));
		}

		public IEnumerable<BlockPos> GetPositions(PlacementContext context, IRandom random, BlockPos origin) {
			yield return origin.AtY(this.heightProvider.Sample(random, context));
		}
	}
}
