using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Main : MonoBehaviour {
	[SerializeField] private GameConfig gameConfig;
	private static Main instance;
	private static Soulbound soulbound;

	private void Awake() => instance = this;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void StartWorld() {
		if (Application.isEditor && SceneManager.GetActiveScene().name != instance?.gameConfig.dev.devScene
				|| (!instance?.gameObject.activeSelf ?? true)) {
			return;
		}
		soulbound = new Soulbound(instance.gameConfig);
		soulbound.Run();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	public static void Preload() {
		AssetManager.PreloadAll();
	}

	private void OnApplicationQuit() {
		soulbound.OnApplicationQuit();
	}

}
