using Cysharp.Threading.Tasks;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.Exceptions;

namespace SoulboundBackend.Client.World {
	public sealed class LevelLoader {
		private readonly ISeedProvider seedProvider;

		public LevelLoader(ISeedProvider seedProvider) {
			this.seedProvider = seedProvider;
		}

		public async UniTask<WorldSession> LoadLevel(UniTask sceneLoadTask, Func<IWorldSceneRoot> rootProvider) {
			await sceneLoadTask;

			IWorldSceneRoot sceneRoot = rootProvider()
				?? throw new OperationException("An error occurred while trying to retrieve the world scene root.");

			Level level = new(seedProvider.GetSeed());
			LevelManager levelManager = new(level);

			// WorldDump? dump param will remain null until serialization is properly implemented
			levelManager.BootstrapWorld(dump: null, sceneRoot.GetGridContext());

			return new WorldSession {
				deserializationData = null,
				level = level,
				levelManager = levelManager,
				player = level.GetPlayer(),
				canvas = sceneRoot.canvas
			};
		}

	}
}
