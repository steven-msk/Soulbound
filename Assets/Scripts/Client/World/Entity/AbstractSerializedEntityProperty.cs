using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using System;

namespace SoulboundBackend.Client.World.EntitySystem {
	public abstract class AbstractSerializedEntityProperty {
		public abstract object GetValueAsObject();
		public abstract void SetValueFromObject(object value);
		public TValue GetValue<TValue>() => (TValue)GetValueAsObject();
		public abstract string GetKey();
	}
}
