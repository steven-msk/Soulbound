using SoulboundBackend.Client;
using SoulboundBackend.Client.UI.Storage;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller {
	[SerializeField] private PlayerController playerPrefab;

	public override void InstallBindings() {
		Container.Bind<PlayerController>().FromComponentInNewPrefab(playerPrefab).AsSingle();

	}
}