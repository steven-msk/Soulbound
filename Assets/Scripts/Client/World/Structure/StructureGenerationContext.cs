using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Chunk;
using System;

namespace SoulboundBackend.Client.World.Structure {
    [Obsolete]
    public readonly struct StructureGenerationContext {
        public readonly int chunkBlockX;
        public readonly int chunkBlockY;
        public readonly int chunkX;
        public readonly Level level;
        public int seed => level.seed;

        public StructureGenerationContext(int chunkBlockX, int chunkBlockY, int chunkX, Level level) {
            this.chunkBlockX = chunkBlockX;
            this.chunkBlockY = chunkBlockY;
            this.chunkX = chunkX;
            this.level = level;
        }
    }
}