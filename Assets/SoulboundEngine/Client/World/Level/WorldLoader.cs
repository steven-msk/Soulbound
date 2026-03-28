using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.Generation;
using System;
using UnityEngine.ResourceManagement.Exceptions;

namespace SoulboundEngine.Client.World.LevelDomain {
	public sealed class WorldLoader {
		private readonly ISeedProvider seedProvider;

		public WorldLoader(ISeedProvider seedProvider) {
			this.seedProvider = seedProvider;
		}

		public async UniTask<WorldSession> LoadWorld(UniTask sceneLoadTask, Func<IWorldSceneRoot> rootProvider) {
			await sceneLoadTask;

			IWorldSceneRoot sceneRoot = rootProvider() ?? throw new OperationException("Root provider returned null");

			BlockRenderer blockRenderer = new(sceneRoot.tilemap);
			BlockModelResolver blockModelResolver = new();
			LevelManager levelManager = new(seedProvider, blockRenderer, blockModelResolver);

			// single level for now
			// multiple dimensions not supported yet
			Level level = levelManager.GetLevel();

			// no deserialization just yet
			// only generation currently
			level.GenerateTerrain();

			levelManager.StartSession();

			return new WorldSession {
				deserializationData = null,
				level = level,
				levelManager = levelManager,
				player = level.GetPlayer(),
				canvas = sceneRoot.canvas,
			};
		}

	}
}
