#nullable enable

using System;

namespace SoulboundEngine.Client.World.Chunk {
	public interface IChunkCache : IDisposable {
		WorldChunk? TryClaim(int chunkX);

		void Tick();

		void Return(WorldChunk chunk);
	}
}
