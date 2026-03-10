using Cysharp.Threading.Tasks;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.Exceptions;

namespace SoulboundBackend.Client.World {
	public sealed class LevelLoader {
		private readonly string worldName;
		private readonly ISeedProvider seedProvider;

		public LevelLoader(string worldName, ISeedProvider seedProvider) {
			this.worldName = worldName;
			this.seedProvider = seedProvider;
		}

		public async UniTask<WorldSession> LoadLevel(UniTask sceneLoadTask, Func<IWorldSceneRoot> rootProvider) {
			await sceneLoadTask;

			IWorldSceneRoot sceneRoot = rootProvider()
				?? throw new OperationException("An error occurred while trying to retrieve the world scene root.");
			sceneRoot.sceneContext.Install();
			sceneRoot.sceneContext.Resolve();

			LevelManager levelManager = sceneRoot.GetLevelManager();

			// WorldDump? dump param will remain null until serialization is properly implemented
			int seed = seedProvider.GetSeed();
			Level level = levelManager.BootstrapWorld(worldName, dump: null, seed, sceneRoot.GetGridContext());

			return new WorldSession {
				deserializationData = null,
				level = level,
				levelManager = levelManager,
				player = level.GetPlayer()
			};
		}

	}
}
