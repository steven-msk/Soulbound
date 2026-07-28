
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.Level;

namespace SoulboundEngine.Client.World.Generation {

	public interface IBiome {
		float GetDensity(int blockX);
		TerrainModulation SampleTerrain(int blockX);
		CaveModulation SampleCave(int blockX, int blockY);

		BlockState ResolveBlock(BlockGenContext ctx);
		void PostProcess(ChunkGenData genData, WorldChunk chunk, Level.Level level, int partitionStartX, int partitionLimitX) {
		}

		BlockState ResolveCaveBlock(BlockPos pos, float density) {
			return Blocks.AIR.DefaultState;
		}
	}
}
