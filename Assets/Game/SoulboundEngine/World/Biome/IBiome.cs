using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Gen;

namespace SoulboundEngine.World.Biome {
	using Chunk = Chunk.Chunk;
	using Level = Level.Level;
	
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
