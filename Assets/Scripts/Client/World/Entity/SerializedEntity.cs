using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	[Obsolete]
	public struct SerializedEntity {
		public Type entityScriptType;
		public Guid id;
		public string prefabDefinitionID;
		public Vector2 lastPosition;
		public List<AbstractSerializedEntityProperty> properties;

		public SerializedEntity(Type entityScriptType, Guid id, string prefabDefinitionID, Vector2 lastPosition, List<AbstractSerializedEntityProperty> properties) {
			this.entityScriptType = entityScriptType;
			this.id = id;
			this.prefabDefinitionID = prefabDefinitionID;
			this.lastPosition = lastPosition;
			this.properties = properties ?? new List<AbstractSerializedEntityProperty>();
		}
	}
}