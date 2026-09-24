namespace SoulboundEngine.World.Serialization {
	using SoulboundEngine.Serialization;

	public struct WorldSave {
		public File saveFolder;
		public File chunksFolder;
		public string name;
		public LevelPropertyInfo levelProperties;
		public bool isNew;

		public readonly int seed => this.levelProperties.seed;

		public WorldSave(File saveFolder, File chunksFolder, string name, LevelPropertyInfo levelProperties) {
			this.saveFolder = saveFolder;
			this.chunksFolder = chunksFolder;
			this.name = name;
			this.levelProperties = levelProperties;
			this.isNew = false;
		}
	}
}
