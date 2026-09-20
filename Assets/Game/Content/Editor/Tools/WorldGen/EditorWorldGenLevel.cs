namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
    using SoulboundEngine.Registry;
    using SoulboundEngine.World.Block;
    using SoulboundEngine.World.Block.Entity;
    using SoulboundEngine.World.Block.State;
    using SoulboundEngine.World.Chunk;
    using SoulboundEngine.World.Entity;
    using SoulboundEngine.World.Gen.Biome;
    using SoulboundEngine.World.Level;
    using SoulboundEngine.World.Physics;
    using SoulboundEngine.World.Player;
    using System;
    using System.Collections.Generic;

    public class EditorWorldGenLevel : IWorldGenLevel {
        private readonly LevelType levelType;
        private readonly long seed;
        private readonly Chunks chunks;
        private readonly IRegistryManager registryManager;

        public EditorWorldGenLevel(LevelType levelType, long seed, int chunkCount, IRegistryManager registryManager, Func<int, Chunk> chunkFactory) {
            this.levelType = levelType;
            this.seed = seed;
            this.registryManager = registryManager;
            this.chunks = new Chunks(chunkCount, chunkFactory);
        }

        public bool AddNewEntity(Entity entity) => false;

        public RegistryEntry<Biome> GetBiome(BlockPos blockPos) {
            Chunk chunk = this.GetChunk(SectionPos.BlockToSectionCoord(blockPos.x), false);
            return chunk.GetBiome(chunk.pos.ToLocalX(blockPos.x));
        }

        public BlockState GetBlockState(BlockPos blockPos) {
            int chunkX = SectionPos.BlockToSectionCoord(blockPos.x);
            return this.GetChunk(chunkX, false).GetBlockState(blockPos);
        }

        public Chunk GetChunk(int chunkX, bool loadOrGenerate) {
            return this.chunks.GetChunk(chunkX, loadOrGenerate);
        }

        public ChunkManager GetChunkManager() => this.chunks;

        public List<Entity> GetEntities(Entity except, AABB box, Predicate<Entity> selector) {
            return new List<Entity>();
        }

        public int GetHeight(int blockX) {
            Chunk chunk = this.GetChunk(SectionPos.BlockToSectionCoord(blockX), false);
            return chunk.GetHeight(chunk.pos.ToLocalX(blockX));
        }

        public LevelType GetLevelType() => this.levelType;

        public PlayerEntity GetPlayer() => null;

        public IRegistryManager GetRegistries() => this.registryManager;

        public long GetSeed() => this.seed;

        public TileEntity GetTileEntity(BlockPos blockPos) {
            return null;
        }

        public bool IsStateAtPosition(BlockPos blockPos, Predicate<BlockState> predicate) {
            return predicate(this.GetBlockState(blockPos));
        }

        public bool RemoveBlock(BlockPos blockPos) {
            return this.SetBlockState(blockPos, Blocks.AIR.DefaultState);
        }

        public void RemoveEntity(Entity entity) {
        }

        public bool SetBlockState(BlockPos blockPos, BlockState blockState) {
            return this.chunks.GetChunk(SectionPos.BlockToSectionCoord(blockPos.x), false).SetBlockState(blockPos, blockState) != null;
        }

        private sealed class Chunks : ChunkManager {
            private readonly int chunkCount;
            private readonly Chunk[] chunks;

            public Chunks(int chunkCount, Func<int, Chunk> chunkFactory) {
                this.chunkCount = chunkCount;
                this.chunks = new Chunk[chunkCount];
                for (int i = 0; i < chunkCount; i++) {
                    this.chunks[i] = chunkFactory(i);
                }
            }

            public override Chunk GetChunk(int x, bool loadOrCreate) {
                return this.chunks[x];
            }

            public override int GetLoadedChunkCount() => this.chunkCount;

            public override void Tick(bool tickChunks) {
            }
        }
    }
}
