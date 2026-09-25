namespace SoulboundEngine.World.Gen.HeightProvider {
	using SoulboundEngine.Common.Math.Random;

	public class ConstantHeight : IHeightProvider {
		public static readonly ConstantHeight ZERO = new(IVerticalAnchor.Absolute(0));
		private readonly IVerticalAnchor value;

		private ConstantHeight(IVerticalAnchor value) {
			this.value = value;
		}

		public static ConstantHeight Of(IVerticalAnchor anchor) => new(anchor);

		public int Sample(IRandom random, WorldGenContext context) {
			return this.value.ResolveY(context);
		}
	}
}
