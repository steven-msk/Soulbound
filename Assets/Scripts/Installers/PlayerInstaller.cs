using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public class PlayerInstaller : MonoInstaller {
		[SerializeField] private bool instantiatePlayer = false;
		public override void InstallBindings() {
			new PlayerInstallerWrapper(instantiatePlayer).InstallBindings(Container);
		}
	}

	public class PlayerInstallerWrapper : AbstractInstallerWrapper {
		private bool instantiatePlayer;

		public PlayerInstallerWrapper(bool instantiatePlayer) {
			this.instantiatePlayer = instantiatePlayer;
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
			container.Bind<InventoryController>().FromComponentInNewPrefab(inventoryPrefab).AsSingle();
			container.Bind<HotbarController>().FromComponentInChildren().AsSingle();
			container.Bind<StepClimber>().FromComponentOnRoot().AsSingle();
			Debug.Log("player installer has installed bindings");
		}
	}
}
