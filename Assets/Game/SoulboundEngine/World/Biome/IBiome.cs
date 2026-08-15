
using SoulboundEngine.Client.World;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.Gen;
using SoulboundEngine.Client.World.Level;

namespace SoulboundEngine.World.Biome {
	public interface IBiome {
		float GetDensity(int blockX);
		TerrainModulation SampleTerrain(int blockX);
		CaveModulation SampleCave(int blockX, int blockY);

		BlockState ResolveBlock(BlockGenContext ctx);
		void PostProcess(ChunkGenData genData, Chunk chunk, Level level, int partitionStartX, int partitionLimitX) {
		}

		BlockState ResolveCaveBlock(BlockPos pos, float density) {
			return Blocks.AIR.DefaultState;
		}
	}
}
