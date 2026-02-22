 using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using UnityEngine;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public class LevelInstaller : InstallerAdapter {
		private readonly Canvas canvas;

		public LevelInstaller(Canvas canvas) {
			this.canvas = canvas;
		}

		public override void InstallBindings(DiContainer container) {
			container.Bind<LevelManager>().FromNewComponentOn(
				new GameObject("LevelManager")
			).AsSingle().NonLazy();

			container.BindInstance(canvas).AsSingle();
			container.BindInstance(Camera.main).AsSingle();
		}
	}
}
