namespace SoulboundEngine.World.Gen.Surface {
	using SoulboundEngine.World.Block;
	using static SoulboundEngine.World.Gen.Surface.SurfaceRules;

	public static class SurfaceRuleData {
		private static readonly IRuleSource AIR = State(Blocks.AIR.DefaultState);
		private static readonly IRuleSource GRASS = State(Blocks.GRASS.DefaultState);
		private static readonly IRuleSource DIRT = State(Blocks.DIRT.DefaultState);
		private static readonly IRuleSource STONE = State(Blocks.STONE.DefaultState);

		public static IRuleSource Default() {
			IRuleSource defaultSurface = Sequence(
				IfTrue(ON_FLOOR, GRASS),
				IfTrue(UnderFloor(4), DIRT),
				STONE
			);
			return Sequence(
				IfTrue(IsBiome(Biome.Biome.HILLS), Sequence(
					IfTrue(Steep, STONE),
					defaultSurface
				)),
				defaultSurface
			);
		}

		public static IRuleSource Air() => AIR;
	}
}
