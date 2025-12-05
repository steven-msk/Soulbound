using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Core;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public abstract class Entity : MonoBehaviour, ISerializable<SerializedEntity> {
		public EntityManager manager { get; private set; }
		public abstract Type scriptType { get; }
		public Guid id { get; private set; }
		public abstract string prefabDefinitionID { get; }
		public Vector2 position => transform.position;
		public abstract Facing facing { get; }

		public void InitState(Guid id, EntityManager entityManager) {
			this.id = id;
			this.manager = entityManager;
		}

		public abstract void ApplySpawnData<TData>(TData spawnData) where TData : ISpawnData;

		public SerializedEntity Serialize() {
			var properties = new SerializedEntityPropertyList();
			foreach (var component in GetComponents<ISerializableComponent>()) {
				component.Save(properties);
			}
			return new SerializedEntity(scriptType, id, prefabDefinitionID, position, properties);
		}

		public void Deserialize(SerializedEntity serialized) {
			this.id = serialized.id;
			this.transform.position = serialized.lastPosition;
			var properties = SerializedEntityPropertyList.From(serialized.properties);
			foreach (var component in GetComponents<ISerializableComponent>()) {
				component.Read(properties);
			}
		}


		//public SerializedEntity Serialize() {
		//	if (manager == null) {
		//		throw new InvalidOperationException("Entity state not initialized");
		//	}
		//	return new(this.entityScriptType, this.id, this.prefabDefinitionID, this.position, this.GetSerializedProperties());
		//}

		//public void Deserialize(SerializedEntity serialized) {
		//	this.id = serialized.id;
		//	var attributes = this.GetType().GetMethod("Spawn")
		//		.GetCustomAttributes<EntitySpawnPropertyCandidatesAttribute>(true);
		//	string[] propertyCandidates = attributes
		//		.SelectMany(attribute => attribute.candidates).Distinct().ToArray();
		//	string[] spawnDataCandidates = propertyCandidates
		//		.Where(c => serialized.properties.Any(p => p.GetKey() == c)).ToArray();
		//	EntitySpawnData spawnData = new(serialized.lastPosition);

		//	foreach (string candidate in spawnDataCandidates) {
		//		var property = serialized.properties.First(p => p.GetKey() == candidate);
		//		spawnData.Set(new SpawnDataKey(candidate), property.ToSpawnDataValue());
		//	}

		//	this.Spawn(spawnData);
		//	this.ApplySerializedProperties(SerializedEntityPropertyList.From(serialized.properties));
		//}
	}

	//[Obsolete]
	//public static class EntityFieldRetriever {
	//	public static Bounds GetBoundsOrFallback(this Entity entity, Vector2 fallbackSize) {
	//		Collider2D collider = entity.GetComponent<Collider2D>();
	//		if (collider != null) {
	//			return collider.bounds;
	//		}
	//		return new Bounds(entity.position, fallbackSize);
	//	}

	//	public static Bounds GetColliderBounds(this Entity entity) {
	//		return entity.GetComponent<Collider2D>().bounds;
	//	}

	//	public static float GetFacingFromXScaleSign(this Entity entity) {
	//		return Mathf.Sign(entity.transform.localScale.x);
	//	}

	//	public static void SetFacingUsingXScaleSign(this Entity entity, float facing) {
	//		facing = Mathf.Sign(facing);
	//		Vector3 scale = entity.transform.localScale;
	//		scale.x = facing;
	//		entity.transform.localScale = scale;
	//		entity.facing = facing;
	//	}
	//}
}
