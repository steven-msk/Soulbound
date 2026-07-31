using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.World.Generation;
using System;
using UnityEngine.ResourceManagement.Exceptions;

namespace SoulboundEngine.Client.World.Level {
	public sealed class WorldLoader {
		private readonly SoulboundClient client;
		private readonly ISeedProvider seedProvider;

		public WorldLoader(SoulboundClient client, ISeedProvider seedProvider) {
			this.client = client;
			this.seedProvider = seedProvider;
		}

		public async UniTask<WorldSession> LoadWorld(UniTask sceneLoadTask, Func<IWorldSceneRoot> rootProvider) {
			await sceneLoadTask;

			IWorldSceneRoot sceneRoot = rootProvider() ?? throw new OperationException("Root provider returned null");

			LevelManager levelManager = new(this.client, this.seedProvider);

			// single level for now
			// multiple dimensions not supported yet
			Level level = levelManager.GetLevel();

			// no deserialization just yet
			// force generation on every load
			level.GenerateTerrain();

			return new WorldSession {
				deserializationData = null,
				level = level,
				levelManager = levelManager,
				canvas = sceneRoot.canvas,
				uiDocument = sceneRoot.UIDocument,
				tilemap = sceneRoot.tilemap
			};
		}

	}
}
