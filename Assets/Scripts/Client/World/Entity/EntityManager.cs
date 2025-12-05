using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.World.EntitySystem {
	public sealed class EntityManager : ISerializable<Dictionary<Guid, SerializedEntity>> {
		private readonly UpdateManager updater;
		private readonly List<IEntitySubsystem> subsystems = new();
		private readonly TickManager tickManager;
		private readonly EntityChunkTracker chunkTracker;
		private readonly Dictionary<Guid, Entity> all = new();

		public EntityManager(Level level, UpdateManager updater) {
			this.updater = updater;

			this.tickManager = new TickManager();
			this.chunkTracker = new EntityChunkTracker(level);

			subsystems.Add(tickManager);
			subsystems.Add(chunkTracker);
			subsystems.Add(updater);
		}

		public void Deserialize(Dictionary<Guid, SerializedEntity> serializedEntities) {
			foreach (var entry in serializedEntities) {
				SerializedEntity serializedEntity = entry.Value;
				Guid id = entry.Key;

				GameObject? entityPrefab = ResourceManager.GetRuntimePrefab(serializedEntity.prefabDefinitionID);
				if (entityPrefab == null) {
					entityPrefab = ResourceManager.Get<GameObject, ResourceGroups.Prefabs>(serializedEntity.prefabDefinitionID);
				}
				Entity entity = (Entity)GameObject.Instantiate(entityPrefab)!.GetComponent(serializedEntity.entityScriptType);

				this.AddEntity(entity, id);
				entity.Deserialize(serializedEntity);
			}
		}

		public Dictionary<Guid, SerializedEntity> Serialize() {
			return all.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Serialize());
		}

		public void AddEntity(Entity entity, Guid? id = null) {
			Guid assigned = id ?? Guid.NewGuid();
			entity.InitState(assigned, this);
			all.Add(assigned, entity);

			foreach (var subsystem in subsystems) {
				subsystem.AddEntity(entity);
			}
		}

		public void Spawn(Entity entity) {
			Guid id = Guid.NewGuid();
			this.AddEntity(entity, id);
		}

		public void Spawn(GameObject prefab) {
			var obj = GameObject.Instantiate(prefab);
			this.Spawn(obj.GetComponent<Entity>());
		}

		public void Spawn<TEntity, TData>(TEntity entity, TData spawnData)
				where TEntity : Entity, IEntitySpawnable<TData> 
				where TData : ISpawnData {
			this.Spawn(entity);
			entity.ApplySpawnData(spawnData);
		}

		public void Spawn<TEntity, TData>(GameObject prefab, TData spawnData)
				where TEntity : Entity, IEntitySpawnable<TData>
				where TData : ISpawnData {
			var obj = GameObject.Instantiate(prefab);
			this.Spawn(obj.GetComponent<TEntity>(), spawnData);
		}

		//public T SpawnSerialized<T>(SerializedEntity serializedEntity, ISpawnData spawnData) where T : Entity {
		//	GameObject prefab = ResourceManager.Get<GameObject, ResourceGroups.Prefabs>(
		//		serializedEntity.prefabDefinitionID
		//	)!;
		//	if (prefab == null) {
		//		throw new ArgumentException($"Entity prefab '{serializedEntity.prefabDefinitionID}' not found");
		//	}

		//	GameObject obj = GameObject.Instantiate(prefab);
		//	T? entity = obj.GetComponent(typeof(T)) as T;
		//	if (entity == null) {
		//		GameObject.Destroy(obj);
		//		throw new MissingComponentException($"No entity component found on prefab {serializedEntity.prefabDefinitionID}");
		//	}

		//	entity.InitState(serializedEntity.id, this);
		//	entity.Deserialize(serializedEntity);
		//	entity.ApplySpawnData(spawnData);
		//	foreach (var subsystem in subsystems) {
		//		subsystem.AddEntity(entity);
		//	}
		//	all.Add(entity.id, entity);

		//	return entity;
		//}

		public void RemoveEntity(Entity entity) {
			foreach (var subsystem in subsystems) {
				subsystem.RemoveEntity(entity);
			}

			all.Remove(entity.id);
			GameObject.Destroy(entity.gameObject);
		}

		public void Tick() => tickManager.Tick();

		public void Update(float deltaTime) {
			foreach (var entity in all.Values) {
				chunkTracker.UpdateEntityChunk(entity);
			}
		}

		public bool GetEntityByID(Guid id, out Entity entity) {
			return all.TryGetValue(id, out entity);
		}
	}
}