
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.LevelDomain;

namespace SoulboundEngine.Client.World.Generation {

	public interface IBiome {
		float GetDensity(int blockX);
		TerrainModulation SampleTerrain(int blockX);
		CaveModulation SampleCave(int blockX, int blockY);

		BlockState ResolveBlock(BlockGenContext ctx);
		void PostProcess(ChunkGenData genData, WorldChunk chunk, Level level, int partitionStartX, int partitionLimitX) {
		}

		BlockState ResolveCaveBlock(BlockPos pos, float density) {
			return Blocks.AIR.DefaultState;
		}
	}
}
