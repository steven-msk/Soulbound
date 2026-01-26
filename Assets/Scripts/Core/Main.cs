using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Main : MonoBehaviour {
	[SerializeField] private GameConfig gameConfig;
	private static Main instance;

	private void Awake() => instance = this;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void StartWorld() {
		if (Application.isEditor && SceneManager.GetActiveScene().name != instance?.gameConfig.dev.devScene
				|| (!instance?.gameObject.activeSelf ?? true)) {
			return;
		}
		new Soulbound(instance.gameConfig).Launch();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	public static void Preload() {
		AssetManager.PreloadAll();
	}
}
