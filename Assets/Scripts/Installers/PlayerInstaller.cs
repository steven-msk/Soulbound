using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller {
	[SerializeField] private InputHandler inputHandlerPrefab;
	[SerializeField] private InventoryController inventoryPrefab;

	public override void InstallBindings() {
		Container.Bind<InputHandler>().FromComponentInNewPrefab(inputHandlerPrefab).AsSingle();
		Container.Bind<PlayerPhysics>().FromComponentOnRoot().AsSingle();
		Container.Bind<ItemUsageHandler>().AsSingle();
		Container.Bind<InventoryController>().FromComponentInNewPrefab(inventoryPrefab).AsSingle();
		Container.Bind<StepClimber>().FromComponentOnRoot().AsSingle();
	}
}