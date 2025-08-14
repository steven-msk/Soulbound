using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class EntityManager {
	private readonly Level level;
	private readonly Dictionary<Guid, Entity> allEntities = new();
	private readonly List<ITickable> tickables = new();
	private readonly List<Entity> pendingRemovals = new();

	public EntityManager(Level level) {
		this.level = level;
	}

	// FIXME: inconsistent chunk-entity relation

	public void AddEntity(Entity entity, Vector2 position) {
		entity.id = Guid.NewGuid();
		allEntities.Add(entity.id, entity);
		if (entity is ITickable tickable) {
			tickables.Add(tickable);
		}
	}

	public void SpawnEntity(Entity entity, EntitySpawnData spawnData) {
		this.AddEntity(entity, spawnData.position);
		entity.Spawn(spawnData);
	}

	public void RemoveEntity(Entity entity) => pendingRemovals.Add(entity);

	public void Tick() {
		tickables.ForEach(tickingEntity => tickingEntity.Tick());
	}

	public void Update(float deltaTime) {
		pendingRemovals.ForEach(entity => {
			allEntities.Remove(entity.id);
			if (entity is ITickable tickable) {
				tickables.Remove(tickable);
			}
			GameObject.Destroy(entity.gameObject);
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

	public void HandleChunkChange(Entity entity, int oldChunkX, int newChunkX) {
		bool newLoaded = level.IsChunkLoaded(newChunkX);
		bool oldLoaded = level.IsChunkLoaded(oldChunkX);
		if (oldLoaded && !newLoaded) {
			entity.OnChunkUnloaded();
		} else if (!oldLoaded && newLoaded) {
			entity.OnChunkLoaded();
		}
	}

#nullable enable

	public HashSet<Entity> GetEntitiesInChunk(WorldChunk chunk) {
		return allEntities.Values
			.Where(entity => ChunkBlockPos.FromWorld(entity.position).underlyingChunk == chunk)
			.ToHashSet();
	}

	public Entity? GetEntityById(Guid id) {
		if (allEntities.TryGetValue(id, out var entity)) {
			return entity;
		}
		return null;
	}
}