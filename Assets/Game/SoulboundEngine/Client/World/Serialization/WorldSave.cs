using SoulboundEngine.Core.Serialization;

namespace SoulboundEngine.Client.World {
	public readonly struct WorldSave {
		public readonly File saveFolder;
		public readonly string name;
		public readonly int seed;
		public readonly bool isNew;

		public WorldSave(File saveFolder, string name, int seed, bool isNew) {
			this.saveFolder = saveFolder;
			this.name = name;
			this.seed = seed;
			this.isNew = isNew;
		}
	}
}
