using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using Zenject;
using System;
using static PlayerInputActions;

namespace SoulboundBackend.Core.Bootstrap {
	public class PlayerInstaller : InstallerAdapter {
		private readonly PlayerController playerInstance;
		private readonly Canvas canvas;
		private readonly InputHandler inputHandler;

		public PlayerInstaller(PlayerController playerInstance, Canvas canvas, InputHandler inputHandler) {
			this.playerInstance = playerInstance;
			this.canvas = canvas;
			this.inputHandler = inputHandler;
		}

		public override void InstallBindings(DiContainer container) {
			GameObject inventoryPrefab = ResourceManager.GetRuntimePrefab("inventory");

			container.BindInterfacesAndSelfTo<InputHandler>().FromInstance(inputHandler).AsSingle();
			container.BindInstance<PlayerController>(playerInstance).AsSingle();
			container.Bind<PlayerPhysics>().FromComponentOn(playerInstance.gameObject).AsSingle();
			container.Bind<ItemUsageHandler>().AsSingle();
			container.Bind<InventoryController>().FromComponentInNewPrefab(inventoryPrefab).UnderTransform(canvas.transform).AsSingle();

		}
	}
}
