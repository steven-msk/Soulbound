using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public interface IBiome {
		float GetDepth(BlockPos pos);
		float GetDensity(BlockPos pos);
		BlockState ResolveBlock(float depth, BlockPos pos);
		void TryPlaceFeature(int cx, WorldChunk chunk, Level level);
	}
}
