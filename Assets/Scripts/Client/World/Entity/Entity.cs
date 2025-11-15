using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Core;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public abstract class Entity : MonoBehaviour, ISerializable<SerializedEntity> {
		protected EntityManager entityManager = null;
		public Guid id { get; private set; }
		public int currentChunkX { get; private set; }
		public abstract Type entityScriptType { get; }
		public abstract float facing { get; set; }
		[Obsolete] public abstract string prefabDefinitionID { get; }
		public Vector2 position => transform.position;

		public void InitState(Guid id, EntityManager entityManager) {
			this.id = id;
			this.entityManager = entityManager;
			currentChunkX = ChunkWorldPos.FromWorld(position).chunkX;
		}

		public abstract void EntityUpdate(float deltaTime);

		public void ManagerUpdate(EntityManager entityManager) {
			ChunkWorldPos currentPos = ChunkWorldPos.FromWorld(this.position);
			if (currentPos.chunkX != currentChunkX) {
				entityManager.HandleChunkChange(this, currentChunkX, currentPos.chunkX);
				currentChunkX = currentPos.chunkX;
			}
		}

		[EntitySpawnPropertyCandidates("position")]
		public virtual void Spawn(EntitySpawnData spawnData) {
			transform.position = spawnData.Get<Vector2>(SpawnDataKeys.position);
		}

		public virtual void Despawn() {
			entityManager.RemoveEntity(this, destroy: true);
		}

		public abstract void OnChunkLoaded();
		public abstract void OnChunkUnloaded();

		public abstract Bounds GetBounds();

		public abstract SerializedEntityPropertyList GetSerializedProperties();

		public abstract void ApplySerializedProperties(SerializedEntityPropertyList properties);

		public SerializedEntity Serialize() {
			if (entityManager == null) {
				throw new InvalidOperationException("Entity state not initialized");
			}
			return new(this.entityScriptType, this.id, this.prefabDefinitionID, this.position, this.GetSerializedProperties());
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

	public static class EntityFieldRetriever {
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

		public static float GetFacingFromXScaleSign(this Entity entity) {
			return Mathf.Sign(entity.transform.localScale.x);
		}

		public static void SetFacingUsingXScaleSign(this Entity entity, float facing) {
			facing = Mathf.Sign(facing);
			Vector3 scale = entity.transform.localScale;
			scale.x = facing;
			entity.transform.localScale = scale;
			entity.facing = facing;
		}
	}
}
