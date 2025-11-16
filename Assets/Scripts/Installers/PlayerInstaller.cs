using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using Zenject;
using System;

namespace SoulboundBackend.Core.Bootstrap {
	public class PlayerInstaller : InstallerAdapter {
		private PlayerController playerInstance;
		private Canvas canvas;

		public PlayerInstaller(PlayerController playerInstance, Canvas canvas) {
			this.playerInstance = playerInstance;
			this.canvas = canvas;
		}

		public override void InstallBindings(DiContainer container) {
			GameObject inputHandlerPrefab = ResourceManager.GetRuntimePrefab("inputHandler");
			GameObject inventoryPrefab = ResourceManager.GetRuntimePrefab("inventory");

			container.BindInstance<PlayerController>(playerInstance).AsSingle();
			container.Bind<InputHandler>().FromComponentInNewPrefab(inputHandlerPrefab.gameObject).AsSingle();
			container.Bind<PlayerPhysics>().FromComponentOn(playerInstance.gameObject).AsSingle();
			container.Bind<ItemUsageHandler>().AsSingle();
			container.Bind<InventoryController>().FromComponentInNewPrefab(inventoryPrefab).UnderTransform(canvas.transform).AsSingle();
		}
	}
}
