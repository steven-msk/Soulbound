namespace SoulboundEngine.Client.World {
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Serialization;

	public sealed class WorldSaveSeedProvider : ISeedProvider {
		private readonly WorldSave worldSave;

		public WorldSaveSeedProvider(WorldSave save) {
			this.worldSave = save;
		}

		public int GetSeed() => this.worldSave.seed;
	}
}
