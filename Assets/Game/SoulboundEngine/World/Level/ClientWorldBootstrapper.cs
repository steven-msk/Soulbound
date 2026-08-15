using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.Gen;

namespace SoulboundEngine.Client.World.Level {
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
			LevelManager levelManager = new(this.client, this.seedProvider, chunkStorage);

			Level level = levelManager.GetLevel();

			level.GenerateSpawn(this.save.isNew);

			return new WorldBootData {
				level = level,
				levelManager = levelManager,
				save = this.save
			};
		}

	}
}
