using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public interface IBiome {
		float GetDepth(int x, int y);
		BlockState ResolveBlock(float depth, int x, int y);
	}
}
