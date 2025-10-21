using SoulboundBackend.Client;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller {
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private GameObject levelManagerPrefab;

	public override void InstallBindings() {
		Container.Bind<WorldManager>().FromInstance(Soulbound.instance.worldManager).AsSingle();
		Container.Bind<LevelManager>().FromComponentInNewPrefab(levelManagerPrefab).AsSingle().NonLazy();
		Container.Bind<PlayerController>().FromComponentInNewPrefab(playerPrefab).AsSingle();
	}
}