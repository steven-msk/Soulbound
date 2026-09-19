namespace SoulboundEngine.UnityClient.World {
	using Cysharp.Threading.Tasks;
	using SoulboundEngine.Recipe;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Serialization;

	public sealed class ClientWorldBootstrapper {
		private readonly ISeedProvider seedProvider;
		private readonly WorldSave save;
		private readonly IRegistryManager registryManager;

		public ClientWorldBootstrapper(ISeedProvider seedProvider, WorldSave save, IRegistryManager registryManager) {
			this.seedProvider = seedProvider;
			this.save = save;
			this.registryManager = registryManager;
		}

		public async UniTask<WorldBootData> LoadWorld(RegistryEntry<LevelType> levelType, LevelSettings levelSettings, RecipeManager recipeManager) {
			ChunkStorage chunkStorage = new(this.save.chunksFolder);
			EntitySerializer entitySerializer = new(this.save);
			LevelManager levelManager = new(levelType, levelSettings, this.registryManager, this.seedProvider, this.save, recipeManager, chunkStorage, entitySerializer);

			return new WorldBootData {
				level = levelManager.Bootstrap(),
				levelManager = levelManager,
				save = this.save
			};
		}

	}
}
