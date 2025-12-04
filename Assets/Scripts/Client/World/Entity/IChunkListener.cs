using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public interface IChunkListener {
		void OnEnteredChunk(WorldChunk chunk);
		void OnLeftChunk(WorldChunk chunk);
	}
}
