namespace SoulboundEngine.World.Level {
	using Cysharp.Threading.Tasks;
	using SoulboundEngine.Client;
	using SoulboundEngine.Client.World;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Serialization;

	public sealed class ClientWorldBootstrapper {
		private readonly SoulboundClient client;
		private readonly ISeedProvider seedProvider;
		private readonly WorldSave save;

		public ClientWorldBootstrapper(SoulboundClient client, ISeedProvider seedProvider, WorldSave save) {
			this.client = client;
			this.seedProvider = seedProvider;
			this.save = save;
		}

		public async UniTask<WorldBootData> LoadWorld() {
			ChunkStorage chunkStorage = new(this.save.chunksFolder);
			EntitySerializer entitySerializer = new(this.save);
			LevelManager levelManager = new(this.client, this.seedProvider, this.save, chunkStorage, entitySerializer);

			return new WorldBootData {
				level = levelManager.Bootstrap(),
				levelManager = levelManager,
				save = this.save
			};
		}

	}
}
