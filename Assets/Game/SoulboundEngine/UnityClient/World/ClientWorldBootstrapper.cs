namespace SoulboundEngine.UnityClient.World {
	using Cysharp.Threading.Tasks;
	using SoulboundEngine.Recipe;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Serialization;

	public sealed class ClientWorldBootstrapper {
		private readonly ISeedProvider seedProvider;
		private readonly WorldSave save;

		public ClientWorldBootstrapper(ISeedProvider seedProvider, WorldSave save) {
			this.seedProvider = seedProvider;
			this.save = save;
		}

		public async UniTask<WorldBootData> LoadWorld(RecipeManager recipeManager) {
			ChunkStorage chunkStorage = new(this.save.chunksFolder);
			EntitySerializer entitySerializer = new(this.save);
			LevelManager levelManager = new(this.seedProvider, this.save, recipeManager, chunkStorage, entitySerializer);

			return new WorldBootData {
				level = levelManager.Bootstrap(),
				levelManager = levelManager,
				save = this.save
			};
		}

	}
}
