using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class EntityManager {
	private readonly Level level;
	private readonly Dictionary<int, HashSet<IEntity>> entitiesByChunk = new();
	private readonly Dictionary<Guid, IEntity> allEntities = new();
	private readonly List<ITickingEntity> tickingEntities = new();
	private readonly List<IEntity> pendingRemovals = new();

	public EntityManager(Level level) {
		this.level = level;
	}

	// FIXME: inconsistent chunk-entity relation

	public void AddEntity(IEntity entity, Vector2 position) {
		entity.id = Guid.NewGuid();
		int chunkX = ChunkBlockPos.FromWorld(position).chunkX;
		if (!entitiesByChunk.TryGetValue(chunkX, out var entities)) {
			entities = new HashSet<IEntity>();
			entitiesByChunk[chunkX] = entities;
		}
		entities.Add(entity);
		allEntities.Add(entity.id, entity);
		if (entity is ITickingEntity tickingEntity) {
			tickingEntities.Add(tickingEntity);
		}
		Debug.Log($"Added entity with id {entity.id}");
	}

	public void RemoveEntity(IEntity entity) => pendingRemovals.Add(entity);

	public void Tick() {
		tickingEntities.ForEach(tickingEntity => tickingEntity.Tick());
	}

	public void Update(float deltaTime) {
		pendingRemovals.ForEach(entity => {
			int chunkX = ChunkBlockPos.FromWorld(entity.position).chunkX;
			if (entitiesByChunk.TryGetValue(chunkX, out var entities) && entities.Contains(entity)) {
				entitiesByChunk[chunkX].Remove(entity);
			}
			allEntities.Remove(entity.id);
			if (entity is ITickingEntity tickingEntity) {
				tickingEntities.Remove(tickingEntity);
			}
		});
		pendingRemovals.Clear();
		foreach (var entity in allEntities.Values) {
			entity.Update(deltaTime);
		}
	}

	public void OnChunkLoaded(WorldChunk chunk) {
		if (entitiesByChunk.TryGetValue(chunk.xpos, out var entities)) {
			foreach (var entity in entities) {
				entity.OnChunkLoaded();
			}
		}
	}

	public void OnChunkUnloaded(WorldChunk chunk) {
		if (entitiesByChunk.TryGetValue(chunk.xpos, out var entities)) {
			foreach (var entity in entities) {
				entity.OnChunkUnloaded();
			}
		}
	}
}