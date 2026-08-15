using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.Chunk {
	using Level = Level.Level;

	public class LevelChunkCache : IChunkCache {
		private readonly Level level;
		private readonly Dictionary<int, CachedEntry> entries = new();
		private readonly int ticksToLive;

		public LevelChunkCache(Level level, int ticksToLive) {
			this.level = level;
			this.ticksToLive = ticksToLive;
		}

		public WorldChunk? TryClaim(int chunkX) {
			if (this.entries.Remove(chunkX, out CachedEntry? entry)) {
				return entry.chunk;
			}
			return null;
		}

		public void Return(WorldChunk chunk) {
			int x = chunk.GetPos().x;
			this.entries[x] = new CachedEntry(chunk, this.ticksToLive);
		}

		public void Tick() {
			List<int>? expired = null;

			foreach ((int x, CachedEntry entry) in this.entries) {
				entry.ticksUntilDrop--;
				if (entry.ticksUntilDrop <= 0) {
					(expired ??= new()).Add(x);
				}
			}
			if (expired == null) return;

			foreach (int x in expired) {
				CachedEntry entry = this.entries[x];
				this.entries.Remove(x);
				this.level.DropChunk(entry.chunk);
			}
		}

		public void Dispose() {
			foreach ((int chunkX, CachedEntry entry) in this.entries) {
				this.level.DropChunk(entry.chunk);
			}
			this.entries.Clear();
		}

		private sealed class CachedEntry {
			public WorldChunk chunk;
			public int ticksUntilDrop;

			public CachedEntry(WorldChunk chunk, int ticksUntilDrop) {
				this.chunk = chunk;
				this.ticksUntilDrop = ticksUntilDrop;
			}
		}
	}
}
