namespace SoulboundEngine.Client {
	using UnityEngine;

	public sealed class Main : MonoBehaviour {
		[SerializeField] private ClientConfig gameConfig;
		public static Main instance { get; private set; }

		private void Awake() => instance = this;

		public ClientConfig GetClientConfig() {
			ClientConfig config = this.gameConfig;
			config.isRunningInEditor = Application.isEditor;
			return config;
		}
	}
}
