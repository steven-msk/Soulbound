using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Render.Entity;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.Generation;
using SoulboundEngine.Client.World.Render;
using System;
using UnityEngine.ResourceManagement.Exceptions;

namespace SoulboundEngine.Client.World.LevelDomain {
	public sealed class WorldLoader {
		private readonly SoulboundClient client;
		private readonly ISeedProvider seedProvider;
		private readonly EntityRenderManager entityRenderManager;

		public WorldLoader(SoulboundClient client, EntityRenderManager entityRenderManager, ISeedProvider seedProvider) {
			this.client = client;
			this.seedProvider = seedProvider;
			this.entityRenderManager = entityRenderManager;
		}

		public async UniTask<WorldSession> LoadWorld(UniTask sceneLoadTask, Func<IWorldSceneRoot> rootProvider) {
			await sceneLoadTask;

			IWorldSceneRoot sceneRoot = rootProvider() ?? throw new OperationException("Root provider returned null");

			BlockRenderer blockRenderer = new(sceneRoot.tilemap);
			BlockModelResolver blockModelResolver = new();
			WorldRenderer worldRenderer = new(LevelManager.simulationView, blockRenderer, blockModelResolver);
			LevelManager levelManager = new(this.client, this.seedProvider, worldRenderer, this.entityRenderManager);

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
