using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.World.Generation;
using SoulboundEngine.Client.World.Serialization;
using System;
using UnityEngine.ResourceManagement.Exceptions;

namespace SoulboundEngine.Client.World.Level {
	public sealed class WorldLoader {
		private readonly SoulboundClient client;
		private readonly ISeedProvider seedProvider;
		private readonly WorldSerializer worldSerializer;
		private readonly WorldSave save;

		public WorldLoader(SoulboundClient client, ISeedProvider seedProvider, WorldSave save, WorldSerializer worldSerializer) {
			this.client = client;
			this.seedProvider = seedProvider;
			this.save = save;
			this.worldSerializer = worldSerializer;
		}

		public async UniTask<WorldSession> LoadWorld(UniTask sceneLoadTask, Func<IWorldSceneRoot> rootProvider) {
			await sceneLoadTask;

			IWorldSceneRoot sceneRoot = rootProvider() ?? throw new OperationException("Root provider returned null");

			LevelManager levelManager = new(this.client, this.seedProvider);

			// single level for now
			// multiple dimensions not supported yet
			Level level = levelManager.GetLevel();

			bool shouldPlaceGeneratedBlocks = this.save.isNew;
			level.GenerateInitialTerrain(shouldPlaceGeneratedBlocks);
			if (!this.save.isNew) {
				this.worldSerializer.Deserialize(levelManager, this.save.saveFolder);
			}

			return new WorldSession {
				save = this.save,
				level = level,
				levelManager = levelManager,
				canvas = sceneRoot.canvas,
				uiDocument = sceneRoot.UIDocument,
				tilemap = sceneRoot.tilemap
			};
		}

	}
}
