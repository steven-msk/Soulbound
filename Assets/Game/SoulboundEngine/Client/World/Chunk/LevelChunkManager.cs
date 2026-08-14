using SoulboundEngine.Client.World.Gen;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable

namespace SoulboundEngine.Client.World.Chunk {
	using Level = Level.Level;

	public class LevelChunkManager : ChunkManager {
		private readonly WorldChunk?[] loadedChunks;
		private readonly EmptyWorldChunk emptyChunk;
		private readonly ChunkGenerator chunkGenerator;
		private readonly Level level;
		private readonly IChunkCache chunkCache;
		private readonly ChunkStorage chunkStorage;
		private readonly int chunkRadius;
		private readonly int loadRange;
		private int centerX;

		public LevelChunkManager(Level level, ChunkGenerator chunkGenerator, int chunkRadius, IChunkCache chunkCache, ChunkStorage chunkStorage) {
			this.level = level;
			this.chunkGenerator = chunkGenerator;
			this.chunkRadius = chunkRadius;
			this.chunkCache = chunkCache;
			this.chunkStorage = chunkStorage;
			this.loadRange = chunkRadius * 2 + 1;
			this.emptyChunk = new EmptyWorldChunk(level, new ChunkPos(0));
			this.loadedChunks = new WorldChunk?[this.loadRange];
		}

		private static bool IsChunkValid(WorldChunk? chunk, int x) {
			if (chunk == null) return false;
			ChunkPos pos = chunk.GetPos();
			return pos.x == x;
		}

		private bool IsInRange(int chunkX) {
			return Math.Abs(chunkX - this.centerX) <= this.chunkRadius;
		}

		private int GetIndex(int x) {
			return ((x % this.loadRange) + this.loadRange) % this.loadRange;
		}

		public override Chunk? GetChunk(int x, bool loadOrGenerate) {
			int index = this.GetIndex(x);
			WorldChunk? chunk = this.loadedChunks[index];
			if (IsChunkValid(chunk, x)) return chunk;

			if (!loadOrGenerate || !this.IsInRange(x)) {
				return this.emptyChunk;
			}

			return this.GenerateChunk(x, true);			
		}

		public void SetCenterX(int centerX) {
			if (this.centerX == centerX) return;
			this.centerX = centerX;

			for (int i = 0; i < this.loadedChunks.Length; i++) {
				WorldChunk? chunk = this.loadedChunks[i];
				if (chunk == null) continue;

				int chunkX = chunk.chunkX;
				if (!this.IsInRange(chunkX)) {
					this.loadedChunks[i] = null;
					this.chunkCache.Return(chunk);
					this.level.OnChunkUnloaded(chunk);
				}
			}

			for (int dx = -this.chunkRadius; dx <= this.chunkRadius; dx++) {
				int chunkX = centerX + dx;
				int index = this.GetIndex(chunkX);
				WorldChunk? chunk = this.loadedChunks[index];
				if (IsChunkValid(chunk, chunkX)) continue;

				WorldChunk newChunk = this.chunkCache.TryClaim(chunkX) ?? this.GenerateChunk(chunkX, true);
				this.loadedChunks[index] = newChunk;
				this.level.OnChunkLoaded(newChunk);
			}
		}

		public void InitialLoad(int centerX, bool placeBlocks) {
			for (int dx = -this.chunkRadius; dx <= this.chunkRadius; dx++) {
				int chunkX = centerX + dx;
				int index = this.GetIndex(chunkX);
				this.GenerateAndLoadChunk(index, chunkX, placeBlocks);
			}
		}

		private WorldChunk GenerateAndLoadChunk(int index, int x, bool placeBlocks) {
			WorldChunk chunk = this.GenerateChunk(x, placeBlocks);
			this.loadedChunks[index] = chunk;
			this.level.OnChunkLoaded(chunk);
			return chunk;
		}

		private WorldChunk GenerateChunk(int x, bool placeBlocks) {
			if (this.chunkStorage.Read(this.level, x) is WorldChunk existing) {
				return existing;
			}
			WorldChunk chunk = new(this.level, new ChunkPos(x));
			this.chunkGenerator.Generate(this.level, chunk, placeBlocks);
			return chunk;
		}

		public override void Tick(bool tickChunks) {
			this.chunkCache.Tick();

			if (!tickChunks) return;

			foreach (WorldChunk? chunk in this.loadedChunks) {
				chunk?.Tick();
			}
		}

		public override int GetLoadedChunkCount() => this.loadedChunks.Length;

		public int GetCenterX() => this.centerX;

		public IEnumerable<Chunk> GetLoadedChunks() => this.loadedChunks.Where(c => c != null)!;

		public override void Dispose() {
			this.chunkCache.Dispose();
			this.chunkStorage.Dispose();
			foreach (var chunk in this.loadedChunks) {
				if (chunk != null) this.level.DropChunk(chunk);
			}
		}
	}
}
