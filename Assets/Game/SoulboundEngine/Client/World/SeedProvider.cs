using SoulboundEngine.Client.World.Generation;

namespace SoulboundEngine.Client.World {
	public sealed class SeedProvider : ISeedProvider {
		private readonly WorldSave worldSave;

		public SeedProvider(WorldSave save) {
			this.worldSave = save;
		}

		public int GetSeed() => this.worldSave.seed;
	}
}
