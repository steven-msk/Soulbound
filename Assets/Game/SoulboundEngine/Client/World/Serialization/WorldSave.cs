using SoulboundEngine.Core.Serialization;

namespace SoulboundEngine.Client.World {
	public readonly struct WorldSave {
		public readonly File saveFolder;
		public readonly File chunksFolder;
		public readonly string name;
		public readonly int seed;
		public readonly bool isNew;

		public WorldSave(File saveFolder, File chunksFolder, string name, int seed, bool isNew) {
			this.saveFolder = saveFolder;
			this.chunksFolder = chunksFolder;
			this.name = name;
			this.seed = seed;
			this.isNew = isNew;
		}
	}
}
