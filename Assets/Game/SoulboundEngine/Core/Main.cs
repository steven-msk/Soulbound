using UnityEngine;

namespace SoulboundEngine.Core {
	public sealed class Main : MonoBehaviour {
		[SerializeField] private GameConfig gameConfig;
		private static Main instance;

		private void Awake() => instance = this;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void GameLaunch() {
			new Soulbound(instance.gameConfig).Launch();
		}
	}
}
