using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public sealed class EntityChunkTracker : IEntitySubsystem {
		private readonly Dictionary<Entity, int> entitiesByChunk = new();
		private readonly Level level;

		public EntityChunkTracker(Level level) {
			this.level = level;
		}

		public void AddEntity(Entity entity) {
			int cx = ChunkWorldPos.FromWorld(entity.position).chunkX;
			entitiesByChunk[entity] = cx;

			if (level.IsChunkLoaded(cx) && !entity.isDeserialized) {
				(entity as IChunkListener)?.OnEnteredChunk(level.ToChunk(cx));
			}
		}

		public void RemoveEntity(Entity entity) {
			entitiesByChunk.Remove(entity);
		}

		public void UpdateEntityChunk(Entity entity) {
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

			listener.OnLeftChunk(level.ToChunk(currentCx));
			listener.OnEnteredChunk(level.ToChunk(nextCx));
		}
	}
}
