namespace SoulboundEngine.World.Gen.HeightProvider {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Common.Math.Random;

	public class UniformHeight : IHeightProvider {
		private readonly IVerticalAnchor minInclusive;
		private readonly IVerticalAnchor maxInclusive;

		public UniformHeight(IVerticalAnchor minInclusive, IVerticalAnchor maxInclusive) {
			this.minInclusive = minInclusive;
			this.maxInclusive = maxInclusive;
		}

		public static UniformHeight Of(IVerticalAnchor minInclusive, IVerticalAnchor maxInclusive) {
			return new UniformHeight(minInclusive, maxInclusive);
		}

		public int Sample(IRandom random, WorldGenContext context) {
			int min = this.minInclusive.ResolveY(context);
			int max = this.maxInclusive.ResolveY(context);
			if (min > max) {
				Logger.LogWarning("Empty height range: ", this);
				return min;
			}
			return Maths.RandomBetweenInclusive(random, min, max);
		}

		public override string ToString() {
			return $"[{this.minInclusive}-{this.maxInclusive}]";
		}
	}
}
