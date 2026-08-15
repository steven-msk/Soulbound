
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Gen;

namespace SoulboundEngine.Client.World.Biome {
	using Chunk = Chunk.Chunk;

	public interface IBiome {
		float GetDensity(int blockX);
		TerrainModulation SampleTerrain(int blockX);
		CaveModulation SampleCave(int blockX, int blockY);

		BlockState ResolveBlock(BlockGenContext ctx);
		void PostProcess(ChunkGenData genData, Chunk chunk, Level.Level level, int partitionStartX, int partitionLimitX) {
		}

		BlockState ResolveCaveBlock(BlockPos pos, float density) {
			return Blocks.AIR.DefaultState;
		}
	}
}
