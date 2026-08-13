using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.World.Gen;
using SoulboundEngine.Client.World.Serialization;

namespace SoulboundEngine.Client.World.Level {
	public sealed class ClientWorldBootstrapper {
		private readonly SoulboundClient client;
		private readonly ISeedProvider seedProvider;
		private readonly WorldSerializer worldSerializer;
		private readonly WorldSave save;

		public ClientWorldBootstrapper(SoulboundClient client, ISeedProvider seedProvider, WorldSave save, WorldSerializer worldSerializer) {
			this.client = client;
			this.seedProvider = seedProvider;
			this.save = save;
			this.worldSerializer = worldSerializer;
		}

		public async UniTask<WorldBootData> LoadWorld() {
			LevelManager levelManager = new(this.client, this.seedProvider);

			// single level for now
			// multiple dimensions not supported yet
			Level level = levelManager.GetLevel();

			bool shouldPlaceGeneratedBlocks = this.save.isNew;
			level.GenerateSpawn(shouldPlaceGeneratedBlocks);
			if (!this.save.isNew) {
				this.worldSerializer.Deserialize(levelManager, this.save.saveFolder);
			}

			return new WorldBootData {
				level = level,
				levelManager = levelManager,
				save = this.save
			};
		}

	}
}
