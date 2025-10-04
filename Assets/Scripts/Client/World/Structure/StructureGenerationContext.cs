using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Chunk;

namespace SoulboundBackend.Client.World.Structure {
    public readonly struct StructureGenerationContext {
        public readonly int chunkBlockX;
        public readonly int chunkX;
        public readonly ChunkHeightmapData heightmapData;
        public readonly Level level;
        public int seed => level.seed;

        public StructureGenerationContext(int chunkBlockX, int chunkX, ChunkHeightmapData heightmapData, Level level) {
            this.chunkBlockX = chunkBlockX;
            this.chunkX = chunkX;
            this.heightmapData = heightmapData;
            this.level = level;
        }
    }
}