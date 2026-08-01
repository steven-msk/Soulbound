using SoulboundEngine.Client.World.Generation;

namespace SoulboundEngine.Client.World {
	public sealed class WorldSaveSeedProvider : ISeedProvider {
		private readonly WorldSave worldSave;

		public WorldSaveSeedProvider(WorldSave save) {
			this.worldSave = save;
		}

		public int GetSeed() => this.worldSave.seed;
	}
}
