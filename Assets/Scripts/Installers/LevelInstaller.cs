using SoulboundBackend.Client;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using UnityEngine;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public class LevelInstaller : MonoInstaller {
		public override void InstallBindings() {
			new LevelInstallerWrapper(() => Soulbound.instance.worldManager).InstallBindings(Container);
		}
	}

	public class LevelInstallerWrapper : AbstractInstallerWrapper {
		public Func<WorldManager> worldManagerSupplier;

		public LevelInstallerWrapper(Func<WorldManager> worldManagerSupplier) {
			this.worldManagerSupplier = worldManagerSupplier;
		}

		public override void InstallBindings(DiContainer container) {
			var levelManagerPrefab = ResourceManager.GetRuntimePrefab("levelManager");
			GameObject playerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");

			container.Bind<WorldManager>().FromInstance(worldManagerSupplier.Invoke()).AsSingle();
			container.Bind<LevelManager>().FromComponentInNewPrefab(levelManagerPrefab).AsSingle().NonLazy();
			container.Bind<PlayerController>().FromComponentInNewPrefab(playerPrefab).AsSingle();
		}
	}
}