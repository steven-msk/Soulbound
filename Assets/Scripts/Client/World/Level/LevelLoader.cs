using Cysharp.Threading.Tasks;
using SoulboundBackend.Client.World.Generation;
using System;
using UnityEngine.ResourceManagement.Exceptions;

namespace SoulboundBackend.Client.World.LevelDomain {
	public sealed class LevelLoader {
		private readonly ISeedProvider seedProvider;

		public LevelLoader(ISeedProvider seedProvider) {
			this.seedProvider = seedProvider;
		}

		public async UniTask<WorldSession> LoadLevel(UniTask sceneLoadTask, Func<IWorldSceneRoot> rootProvider) {
			await sceneLoadTask;

			IWorldSceneRoot sceneRoot = rootProvider() ?? throw new OperationException("Root provider returned null");

			Level level = new(seedProvider.GetSeed());
			LevelManager levelManager = new(level);

			// WorldDump? dump param will remain null until serialization is properly implemented
			levelManager.BootstrapWorld(dump: null, sceneRoot.GetGridContext());

			return new WorldSession {
				deserializationData = null,
				level = level,
				levelManager = levelManager,
				player = level.GetPlayer(),
				canvas = sceneRoot.canvas,
				audioSource = sceneRoot.audioSource
			};
		}

	}
}
