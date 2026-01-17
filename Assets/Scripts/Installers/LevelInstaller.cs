 using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Core.Resource;
using System;
using UnityEngine;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public class LevelInstaller : InstallerAdapter {
		private WorldManager worldManager;
		private Canvas canvas;

		public LevelInstaller(WorldManager worldManager, Canvas canvas) {
			this.worldManager = worldManager;
			this.canvas = canvas;
		}

		public override void InstallBindings(DiContainer container) {
			var levelManagerPrefab = ResourceManager.GetRuntimePrefab("levelManager");

			container.BindInstance<PlayerInputActions>(Soulbound.instance?.playerInputActions ?? new PlayerInputActions()).AsSingle().NonLazy();
			container.BindInstance<WorldManager>(worldManager).AsSingle();
			container.Bind<LevelManager>().FromComponentInNewPrefab(levelManagerPrefab).AsSingle().NonLazy();
			container.BindInstance<Canvas>(canvas).AsSingle();
			container.BindInstance<Camera>(Camera.main).AsSingle();
			container.Bind<UIManager>().FromComponentOn(canvas.gameObject).AsSingle();
		}
	}
}