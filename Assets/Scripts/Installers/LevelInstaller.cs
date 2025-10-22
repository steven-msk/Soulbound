using SoulboundBackend.Client;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller {
	[SerializeField] private GameObject levelManagerPrefab;

	public override void InstallBindings() => InstallBindings(Container);

	public void InstallBindings(DiContainer container) {
		container.Bind<WorldManager>().FromInstance(Soulbound.instance.worldManager).AsSingle();
		container.Bind<LevelManager>().FromComponentInNewPrefab(levelManagerPrefab).AsSingle().NonLazy();
		InstallPlayerBinding(container);
	}

	public void InstallPlayerBinding(DiContainer container) {
		GameObject playerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");
		container.Bind<PlayerController>().FromComponentInNewPrefab(playerPrefab).AsSingle();
	}
}