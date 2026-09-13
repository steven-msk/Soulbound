namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.World.Block;
	using static SoulboundEngine.World.Gen.SurfaceRules;

	public static class SurfaceRuleData {
		private static readonly IRuleSource GRASS = State(Blocks.GRASS.DefaultState);
		private static readonly IRuleSource DIRT = State(Blocks.DIRT.DefaultState);
		private static readonly IRuleSource STONE = State(Blocks.STONE.DefaultState);

		public static IRuleSource Default() {
			return Sequence(
				IfTrue(AT_SURFACE, GRASS),
				IfTrue(DepthCheck(4), DIRT),
				STONE
			);
		}
	}
}
