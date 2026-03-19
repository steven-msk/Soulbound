using SoulboundBackend.Core;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulboundBackend.Core {
	public sealed class Main : MonoBehaviour {
		[SerializeField] private GameConfig gameConfig;
		private static Main instance;

		private void Awake() => instance = this;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void GameLaunch() {
			if (Application.isEditor && SceneManager.GetActiveScene().name != instance?.gameConfig.dev.devScene
					|| (!instance?.gameObject.activeSelf ?? true)) {
				return;
			}
			new Soulbound(instance.gameConfig).Launch();
		}
	}
}
