using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public sealed class EntityChunkTracker : IEntitySubsystem {
		private readonly Dictionary<Entity_OLD, int> entitiesByChunk = new();
		private readonly Level level;

		public EntityChunkTracker(Level level) {
			this.level = level;
		}

		public void AddEntity(Entity_OLD entity) {
			int cx = ChunkWorldPos.FromWorld(entity.position).chunkX;
			entitiesByChunk[entity] = cx;

			if (level.IsChunkLoaded(cx)) {
				(entity as IChunkListener)?.OnEnteredChunk(level.GetChunk(cx));
			}
		}

		public void RemoveEntity(Entity_OLD entity) {
			entitiesByChunk.Remove(entity);
		}

		public void UpdateEntityChunk(Entity_OLD entity) {
			if (!entitiesByChunk.TryGetValue(entity, out int currentCx)) {
				return;
			}
			int nextCx = ChunkWorldPos.FromWorld(entity.position).chunkX;
			if (currentCx == nextCx) {
				return;
			}

			entitiesByChunk[entity] = nextCx;
			var listener = entity as IChunkListener;
			if (listener == null) {
				return;
			}

			listener.OnLeftChunk(level.GetChunk(currentCx));
			listener.OnEnteredChunk(level.GetChunk(nextCx));
		}
	}
}
