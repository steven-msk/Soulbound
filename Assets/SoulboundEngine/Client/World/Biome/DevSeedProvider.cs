using SoulboundEngine.Common;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.World.Generation {
	[PROTOTYPICAL]
	public sealed class DevSeedProvider : ISeedProvider {
		private readonly DevConfig devConfig;

		public DevSeedProvider(DevConfig devConfig) {
			this.devConfig = devConfig;
		}

		public int GetSeed() => devConfig.seed;
	}
}
