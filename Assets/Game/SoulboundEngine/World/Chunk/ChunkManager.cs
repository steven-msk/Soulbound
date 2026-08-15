
#nullable enable

using System;

namespace SoulboundEngine.World.Chunk {
	public abstract class ChunkManager : IDisposable {

		public abstract Chunk? GetChunk(int x, bool loadOrCreate);

		public WorldChunk? GetWorldChunk(int x, bool loadOrCreate) {
			return this.GetChunk(x, loadOrCreate) as WorldChunk;
		}

		public virtual WorldChunk? GetChunkNow(int x) => this.GetWorldChunk(x, false);

		public bool HasChunk(int x) => this.GetChunk(x, false) != null;

		public virtual void OnSectionStatusChanged(int x, int sectionY, bool previouslyEmpty) {
		}

		public virtual bool SetChunkForced(ChunkPos pos, bool forced) {
			return false;
		}

		public abstract void Tick(bool tickChunks);

		public abstract int GetLoadedChunkCount();

		public virtual void Dispose() {
		}
	}
}
