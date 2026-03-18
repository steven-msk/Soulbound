using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World {
	[PROTOTYPICAL]
	public sealed class DevSeedProvider : ISeedProvider {
		private readonly DevConfig devConfig;

		public DevSeedProvider(DevConfig devConfig) {
			this.devConfig = devConfig;
		}

		public int GetSeed() => devConfig.seed;
	}
}
