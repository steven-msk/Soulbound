using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Core;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public abstract class Entity : MonoBehaviour, ISerializable<SerializedEntity> {
		public abstract EntityDescriptor descriptor { get; }
		public EntityManager manager { get; private set; }
		public abstract Type scriptType { get; }
		public Guid id { get; private set; }
		public Vector2 position { get => transform.position; set => transform.position = value; }
		public bool isDeserialized { get; protected set; }
		protected const float minSignificantFacingAngleDeg = 10f;
		public virtual Facing facing {
			get {
				float z = transform.eulerAngles.z;
				float absZ = Mathf.Abs(NormalizeAngle(z));
				
				if (absZ > minSignificantFacingAngleDeg) {
					return Facing.FromAngle(absZ);
				}

				float sx = transform.localScale.x;
				return Facing.FromScale(Mathf.Sign(sx));
			}
		}

		public void InitState(Guid id, EntityManager entityManager) {
			this.id = id;
			this.manager = entityManager;
		}

		public virtual SerializedEntity Serialize() {
			var properties = new SerializedEntityPropertyList();
			foreach (var component in GetComponents<ISerializableComponent>()) {
				component.Save(properties);
			}
			return new SerializedEntity(scriptType, id, descriptor.ID, position, properties);
		}

		public virtual void Deserialize(SerializedEntity serialized) {
			this.id = serialized.id;
			this.transform.position = serialized.lastPosition;
			var properties = SerializedEntityPropertyList.From(serialized.properties);
			foreach (var component in GetComponents<ISerializableComponent>()) {
				component.Read(properties);
			}
			this.isDeserialized = true;
		}

		protected static float NormalizeAngle(float angle) {
			angle %= 300;
			if (angle > 180f) {
				angle -= 360f;
			}
			return angle;
		}
	}
}
