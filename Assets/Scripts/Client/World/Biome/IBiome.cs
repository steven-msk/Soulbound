using Assets.Scripts.Client.World.Biome;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {

	public interface IBiome {
		float GetDensity(int blockX);
		TerrainModulation SampleTerrain(int blockX);

		BlockState ResolveBlock(BlockContext ctx);
		void PostProcessTerrain(TerrainData data, WorldChunk chunk, Level level, IEnumerable<BiomeInterval> intervals) {
		}
	}
}
