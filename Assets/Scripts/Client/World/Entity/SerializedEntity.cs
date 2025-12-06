using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public struct SerializedEntity {
		public Type entityScriptType;
		public Guid id;
		public string descriptorID;
		public Vector2 lastPosition;
		public List<AbstractSerializedEntityProperty> properties;

		public SerializedEntity(Type entityScriptType, Guid id, string descriptorID, Vector2 lastPosition, List<AbstractSerializedEntityProperty> properties) {
			this.entityScriptType = entityScriptType;
			this.id = id;
			this.descriptorID = descriptorID;
			this.lastPosition = lastPosition;
			this.properties = properties ?? new List<AbstractSerializedEntityProperty>();
		}
	}
}