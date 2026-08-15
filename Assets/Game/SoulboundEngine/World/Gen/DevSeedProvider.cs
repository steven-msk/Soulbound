using SoulboundEngine.Common;

namespace SoulboundEngine.World.Gen {
	[PROTOTYPICAL]
	public sealed class DevSeedProvider : ISeedProvider {
		private readonly DevConfig devConfig;

		public DevSeedProvider(DevConfig devConfig) {
			this.devConfig = devConfig;
		}

		public int GetSeed() => this.devConfig.seed;
	}
}
