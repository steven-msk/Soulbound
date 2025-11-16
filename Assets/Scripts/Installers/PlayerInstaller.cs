using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using Zenject;
using System;

namespace SoulboundBackend.Core.Bootstrap {
	public class PlayerInstaller : AbstractInstaller {
		private bool instantiatePlayer;
		private Canvas canvas;

		public PlayerInstaller(bool instantiatePlayer, Canvas canvas) {
			this.instantiatePlayer = instantiatePlayer;
			this.canvas = canvas;
			Debug.Log("player installer wrapper created");
		}

		public override void InstallBindings(DiContainer container) {
			GameObject inputHandlerPrefab = ResourceManager.GetRuntimePrefab("inputHandler");
			GameObject inventoryPrefab = ResourceManager.GetRuntimePrefab("inventory");

			if (instantiatePlayer) {
				GameObject playerPrefab = ResourceManager.GetRuntimePrefab("player");
				container.Bind<PlayerController>().FromComponentInNewPrefab(playerPrefab).AsSingle();
			} else {
				container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
			}
			container.Bind<InputHandler>().FromComponentInNewPrefab(inputHandlerPrefab.gameObject).AsSingle();
			container.Bind<PlayerPhysics>().FromComponentOnRoot().AsSingle();
			container.Bind<ItemUsageHandler>().AsSingle();
			container.Bind<InventoryController>().FromComponentInNewPrefab(inventoryPrefab).UnderTransform(canvas.transform).AsSingle();
			Debug.Log("player installer has installed bindings");
		}
	}
}
