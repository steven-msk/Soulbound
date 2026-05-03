using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Render.Block;
using SoulboundEngine.Client.Render.Entity;
using SoulboundEngine.Client.World.Generation;
using SoulboundEngine.Client.World.Render;
using System;
using UnityEngine.ResourceManagement.Exceptions;

namespace SoulboundEngine.Client.World.LevelDomain {
	public sealed class WorldLoader {
		private readonly SoulboundClient client;
		private readonly ISeedProvider seedProvider;
		private readonly EntityRenderManager entityRenderManager;
		private readonly BlockRenderManager blockRenderManager;

		public WorldLoader(SoulboundClient client, EntityRenderManager entityRenderManager, BlockRenderManager blockRenderManager, ISeedProvider seedProvider) {
			this.client = client;
			this.seedProvider = seedProvider;
			this.entityRenderManager = entityRenderManager;
			this.blockRenderManager = blockRenderManager;
		}

		public async UniTask<WorldSession> LoadWorld(UniTask sceneLoadTask, Func<IWorldSceneRoot> rootProvider) {
			await sceneLoadTask;

			IWorldSceneRoot sceneRoot = rootProvider() ?? throw new OperationException("Root provider returned null");

			WorldRenderer worldRenderer = new(LevelManager.simulationView, this.blockRenderManager, sceneRoot.tilemap);
			LevelManager levelManager = new(this.client, this.seedProvider, worldRenderer, this.entityRenderManager);

			// single level for now
			// multiple dimensions not supported yet
			Level level = levelManager.GetLevel();
			worldRenderer.SetBlockStateSupplier(level.GetBlockState);

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
