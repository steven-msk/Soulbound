namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.UnityClient;
	using SoulboundEngine.Common;

	[PROTOTYPICAL]
	public sealed class DevSeedProvider : ISeedProvider {
		private readonly UnityClientConfig.Dev devConfig;

		public DevSeedProvider(UnityClientConfig.Dev devConfig) {
			this.devConfig = devConfig;
		}

		public int GetSeed() => this.devConfig.seed;
	}
}
