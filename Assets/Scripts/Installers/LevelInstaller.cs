 using SoulboundBackend.Client;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public class LevelInstaller : AbstractInstaller {
		public readonly WorldManager worldManager;
		public readonly Canvas canvas;

		public LevelInstaller(WorldManager worldManager, Canvas canvas) {
			this.worldManager = worldManager;
			this.canvas = canvas;
		}

		public override void InstallBindings(DiContainer container) {
			var levelManagerPrefab = ResourceManager.GetRuntimePrefab("levelManager");

			container.BindInstance<WorldManager>(worldManager).AsSingle();
			container.Bind<LevelManager>().FromComponentInNewPrefab(levelManagerPrefab).AsSingle().NonLazy();
			container.BindInstance<Canvas>(canvas).AsSingle();
		}
	}
}