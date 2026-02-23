using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using Zenject;
using System;
using SoulboundBackend.Client.Concurrency;
using SoulboundBackend.Core.AssetManagement;

namespace SoulboundBackend.Core.Bootstrap {
	public class PlayerInstaller : InstallerAdapter {
		private readonly PlayerController playerInstance;

		public PlayerInstaller(PlayerController playerInstance) {
			this.playerInstance = playerInstance;
		}

		public override void InstallBindings(DiContainer container) {
			container.BindInterfacesAndSelfTo<ConcurrentActionResolver>().AsSingle();
			container.BindInstance<PlayerController>(playerInstance).AsSingle();
			container.Bind<PlayerPhysics>().FromComponentOn(playerInstance.gameObject).AsSingle();
			container.Bind<ItemUsageHandler>().AsSingle();
		}
	}
}
