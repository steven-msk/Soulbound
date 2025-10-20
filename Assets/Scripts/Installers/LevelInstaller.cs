using SoulboundBackend.Client;
using SoulboundBackend.Client.UI.Storage;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller {
	public override void InstallBindings() {
		Container.Bind<InventoryController>().AsSingle();
		Container.Bind<PlayerController>().AsSingle();
	}
}