using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Entity.SpawnData;
using SoulboundBackend.Core;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SoulboundBackend.Client.World.Entity {
	public abstract class Entity : MonoBehaviour, ISerializable<SerializedEntity> {
		public Guid? id { get; set; } = null;
		public Vector2 position => transform.position;
		public int currentChunkX { get; private set; }
		public abstract Type entityScriptType { get; }
		public abstract string prefabDefinitionID { get; }

		public abstract void EntityUpdate(float deltaTime);

		protected virtual void Start() {
			currentChunkX = ChunkWorldPos.FromWorld(position).chunkX;
		}

		protected virtual void Update() {
			ChunkWorldPos currentPos = ChunkWorldPos.FromWorld(this.position);
			if (currentPos.chunkX != currentChunkX) {
				Soulbound.instance.GetActiveLevel().EntityManager.HandleChunkChange(this, currentChunkX, currentPos.chunkX);
				currentChunkX = currentPos.chunkX;
			}
		}

		[EntitySpawnPropertyCandidates("position")]
		public virtual void Spawn(EntitySpawnData spawnData) {
			transform.position = spawnData.Get<Vector2>(SpawnDataKeys.position);
		}

		public virtual void Despawn() {
			Soulbound.instance.GetActiveLevel().EntityManager.RemoveEntity(this, destroy: true);
		}

		public abstract void OnChunkLoaded();
		public abstract void OnChunkUnloaded();

		public abstract Bounds GetBounds();

		public abstract SerializedEntityPropertyList GetSerializedProperties();

		public abstract void ApplySerializedProperties(SerializedEntityPropertyList properties);

		public SerializedEntity Serialize() {
			return new(this.entityScriptType, this.id.Value, this.prefabDefinitionID, this.position, this.GetSerializedProperties());
		}

		public void Deserialize(SerializedEntity serialized) {
			this.id = serialized.id;
			var attributes = this.GetType().GetMethod("Spawn")
				.GetCustomAttributes<EntitySpawnPropertyCandidatesAttribute>(true);
			string[] propertyCandidates = attributes
				.SelectMany(attribute => attribute.candidates).Distinct().ToArray();
			string[] spawnDataCandidates = propertyCandidates
				.Where(c => serialized.properties.Any(p => p.GetKey() == c)).ToArray();
			EntitySpawnData spawnData = new(serialized.lastPosition);

			foreach (string candidate in spawnDataCandidates) {
				var property = serialized.properties.First(p => p.GetKey() == candidate);
				spawnData.Set(new SpawnDataKey(candidate), property.ToSpawnDataValue());
			}

			this.Spawn(spawnData);
			this.ApplySerializedProperties(SerializedEntityPropertyList.From(serialized.properties));
		}
	}

	public static class EnityBounds {
		public static Bounds GetBoundsOrFallback(this Entity entity, Vector2 fallbackSize) {
			Collider2D collider = entity.GetComponent<Collider2D>();
			if (collider != null) {
				return collider.bounds;
			}
			return new Bounds(entity.position, fallbackSize);
		}

		public static Bounds GetColliderBounds(this Entity entity) {
			return entity.GetComponent<Collider2D>().bounds;
		}
	}
}
