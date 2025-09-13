using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulboundBackend.Client.World.Entity {
	public sealed class EntityManager {
		private readonly Level level;
		private PlayerController player;
		private Dictionary<Guid, Entity> allEntities = new();
		private List<ITickable> tickables = new();
		private List<(Entity entity, bool destroy)> pendingRemovals = new();

		public Dictionary<Guid, Entity> AllExistingEntities => allEntities;

		public EntityManager(Level level) {
			this.level = level;
		}

		public void Boostrap(Dictionary<Guid, SerializedEntity> serializedEntities) {
			foreach (var serializedEntity in serializedEntities.Values) {
				GameObject entityPrefab = ResourceManager.Get<GameObject, ResourceGroups.Prefabs>(serializedEntity.prefabDefinitionID);
				Entity entity = (Entity)GameObject.Instantiate(entityPrefab).GetComponent(serializedEntity.entityScriptType);
				entity.Deserialize(serializedEntity);
				this.AddExistingEntity(entity);
			}
		}

		public void AddExistingEntity(Entity entity) {
			entity.id = Guid.NewGuid();
			allEntities.Add(entity.id.Value, entity);
			if (entity is ITickable tickable) {
				tickables.Add(tickable);
			}
		}

		public void SpawnEntity(Entity entity, EntitySpawnData spawnData) {
			entity.Spawn(spawnData);
			this.AddExistingEntity(entity);
		}

		public void SpawnPlayer(PlayerController player, SerializedEntity serialized) {
			player.Deserialize(serialized);
			this.player = player;
		}

		public void RemoveEntity(Entity entity, bool destroy) => pendingRemovals.Add((entity, destroy));

		public void RemoveEntityImmediately(Entity entity, bool destroy) {
			allEntities.Remove(entity.id.Value);
			if (entity is ITickable tickable) {
				tickables.Remove(tickable);
			}
			if (destroy) {
				GameObject.Destroy(entity.gameObject);
			}
		}

		public void Tick() {
			tickables.ForEach(tickingEntity => tickingEntity.Tick());
		}

		public void Update(float deltaTime) {
			pendingRemovals.ForEach(entry => RemoveEntityImmediately(entry.entity, entry.destroy));
			pendingRemovals.Clear();
			foreach (var entity in allEntities.Values) {
				entity.EntityUpdate(deltaTime);
			}
			this.player.EntityUpdate(deltaTime);
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
			var entities = allEntities.Values.Where(entity => ChunkBlockPos.FromWorld(entity.position).underlyingChunk == chunk);
			return System.Linq.Enumerable.ToHashSet(entities);
		}

		public Entity? GetEntityById(Guid id) {
			if (allEntities.TryGetValue(id, out var entity)) {
				return entity;
			}
			return null;
		}
	}
}