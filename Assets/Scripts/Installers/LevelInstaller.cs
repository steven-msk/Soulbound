 using SoulboundBackend.Client;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public class LevelInstaller : AbstractInstaller {
		public readonly WorldManager worldManager;

		public LevelInstaller(WorldManager worldManager) {
			this.worldManager = worldManager;
		}

		public override void InstallBindings(DiContainer container) {
			var levelManagerPrefab = ResourceManager.GetRuntimePrefab("levelManager");

			container.Bind<WorldManager>().FromInstance(worldManager).AsSingle();
			container.Bind<LevelManager>().FromComponentInNewPrefab(levelManagerPrefab).AsSingle().NonLazy();
		}
	}
}