namespace SoulboundEngine.World.Level {
	using SoulboundEngine.Registry;
	using System.Collections.Generic;

	public static class LevelTypes {
		public static IEnumerable<LevelType> Init() {
			yield return Registry<LevelType>.Register(Registries.LEVEL_TYPE, LevelType.DEFAULT, new LevelType(Level.MIN_Y, Level.MAX_Y));
		}
	}
}
