using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class EntityManager {
	private readonly Level level;
	private readonly Dictionary<Guid, Entity> allEntities = new();
	private readonly List<TickingEntity> tickingEntities = new();
	private readonly List<Entity> pendingRemovals = new();

	public EntityManager(Level level) {
		this.level = level;
	}

	// FIXME: inconsistent chunk-entity relation

	public void AddEntity(Entity entity, Vector2 position) {
		entity.id = Guid.NewGuid();
		allEntities.Add(entity.id, entity);
		if (entity is TickingEntity tickingEntity) {
			tickingEntities.Add(tickingEntity);
		}
	}

	public void RemoveEntity(Entity entity) => pendingRemovals.Add(entity);

	public void Tick() {
		tickingEntities.ForEach(tickingEntity => tickingEntity.Tick());
	}

	public void Update(float deltaTime) {
		pendingRemovals.ForEach(entity => {
			allEntities.Remove(entity.id);
			if (entity is TickingEntity tickingEntity) {
				tickingEntities.Remove(tickingEntity);
			}
		});
		pendingRemovals.Clear();
		foreach (var entity in allEntities.Values) {
			entity.EntityUpdate(deltaTime);
		}
	}

	public void OnChunkLoaded(WorldChunk chunk) {
		foreach (var entity in GetEntitiesInChunk(chunk)) {
			entity.OnChunkLoaded();
		}
	}

	public void OnChunkUnloaded(WorldChunk chunk) {
		foreach (var entity in GetEntitiesInChunk(chunk)) {
			entity.OnChunkUnloaded();
		}
	}

	public HashSet<Entity> GetEntitiesInChunk(WorldChunk chunk) {
		return allEntities.Values
			.Where(entity => ChunkBlockPos.FromWorld(entity.position).underlyingChunk == chunk)
			.ToHashSet();
	}
}