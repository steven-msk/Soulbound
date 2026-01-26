using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.World.EntitySystem {
	public sealed class EntityManager : ISerializable<Dictionary<Guid, SerializedEntity>> {
		private readonly List<IEntitySubsystem> subsystems = new();
		private readonly EntityTickManager tickManager;
		private readonly EntityChunkTracker chunkTracker;
		private readonly Dictionary<Guid, Entity> all = new();

		public EntityManager(Level level, UpdateManager updater) {
			this.tickManager = new EntityTickManager();
			this.chunkTracker = new EntityChunkTracker(level);

			subsystems.Add(tickManager);
			subsystems.Add(chunkTracker);
			subsystems.Add(updater);
		}

		public void Deserialize(Dictionary<Guid, SerializedEntity> serializedEntities) {
			foreach (var entry in serializedEntities) {
				SerializedEntity serializedEntity = entry.Value;
				Guid id = entry.Key;

				var descriptor = EntityDescriptorRegistry.ByID(serializedEntity.descriptorID);
				Entity entity = descriptor.CreateInstance();

				entity.Deserialize(serializedEntity);
				this.AddEntity(entity, id);
			}
		}

		public Dictionary<Guid, SerializedEntity> Serialize() {
			return all.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Serialize());
		}

		public void AddEntity(Entity entity, Guid? id = null) {
			Guid assigned = id ?? Guid.NewGuid();
			entity.InitState(assigned, this);
			all.Add(assigned, entity);

			AddToSubsystems(entity);
		}

		public void Spawn(Entity entity) {
			Guid id = Guid.NewGuid();
			this.AddEntity(entity, id);
		}

		public Entity Spawn(EntityDescriptor descriptor) {
			var entity = descriptor.CreateInstance();
			this.Spawn(entity);
			return entity;
		}

		public Entity Spawn<TEntity, TData>(EntityDescriptor descriptor, TData spawnData)
				where TEntity : Entity, IEntitySpawnable<TData>
				where TData : ISpawnData {
			TEntity entity = (TEntity)descriptor.CreateInstance();
			this.Spawn(entity, spawnData);
			return entity;
		}

		public void Spawn<TEntity, TData>(TEntity entity, TData spawnData)
				where TEntity : Entity, IEntitySpawnable<TData> 
				where TData : ISpawnData {
			entity.ApplySpawnData(spawnData);
			this.Spawn(entity);
		}

		public T SpawnSerialized<T>(SerializedEntity serializedEntity) where T : Entity {
			var descriptor = EntityDescriptorRegistry.ByID(serializedEntity.descriptorID);
			var entity = descriptor.CreateInstance();

			entity.InitState(serializedEntity.id, this);
			entity.Deserialize(serializedEntity);
			AddToSubsystems(entity);

			all.Add(entity.id, entity);
			return (T)entity;
		}

		public TEntity SpawnSerialized<TEntity, TData>(SerializedEntity serializedEntity, TData spawnData)
				where TEntity : Entity, IEntitySpawnable<TData>
				where TData : ISpawnData {
			var descriptor = EntityDescriptorRegistry.ByID(serializedEntity.descriptorID);
			var entity = (TEntity)descriptor.CreateInstance();

			entity.InitState(serializedEntity.id, this);
			entity.Deserialize(serializedEntity);
			entity.ApplySpawnData(spawnData);
			AddToSubsystems(entity);

			all.Add(entity.id, entity);
			return entity;
		}

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

		private void AddToSubsystems(Entity entity) {
			foreach (var subsystem in subsystems) {
				subsystem.AddEntity(entity);
			}
		}

		public void AddSubsystem(IEntitySubsystem subsystem) {
			this.subsystems.Add(subsystem);
		}
	}
}
