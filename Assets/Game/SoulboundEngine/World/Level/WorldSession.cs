namespace SoulboundEngine.World.Level {
	using SoulboundEngine.World.Serialization;

	public partial struct WorldSession {
		public WorldSave save;
		public LevelManager levelManager;
		public Level level;
	}
}
