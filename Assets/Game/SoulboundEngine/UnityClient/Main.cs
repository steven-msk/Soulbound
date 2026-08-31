namespace SoulboundEngine.UnityClient {
	using UnityEngine;

	public sealed class Main : MonoBehaviour {
		[SerializeField] private UnityClientConfig gameConfig;
		public static Main instance { get; private set; }

		private void Awake() => instance = this;

		public UnityClientConfig GetUnityClientConfig() {
			UnityClientConfig config = this.gameConfig;
			config.isRunningInEditor = Application.isEditor;
			return config;
		}
	}
}
