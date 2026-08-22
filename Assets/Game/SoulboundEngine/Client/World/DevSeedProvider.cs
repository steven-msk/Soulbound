namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Client;
	using SoulboundEngine.Common;

	[PROTOTYPICAL]
	public sealed class DevSeedProvider : ISeedProvider {
		private readonly ClientConfig.Dev devConfig;

		public DevSeedProvider(ClientConfig.Dev devConfig) {
			this.devConfig = devConfig;
		}

		public int GetSeed() => this.devConfig.seed;
	}
}
