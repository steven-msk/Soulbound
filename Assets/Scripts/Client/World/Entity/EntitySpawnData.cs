using SoulboundBackend.Client.World.Entity.SpawnData;
using System.Collections.Generic;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Client.World.Entity {
	public class EntitySpawnData : Dictionary<SpawnDataKey, SpawnDataValue> {
		private static readonly Logger logger = Logger.CreateInstance();

		public EntitySpawnData(Vector2 position) {
			this.Add(SpawnDataKeys.position, new SpawnDataValue<Vector2>(position));
		}

		public TValue Get<TValue>(SpawnDataKey key, TValue defaultValue = default) {
			if (this.TryGetValue(key, out var value) && value is SpawnDataValue<TValue> typedValue) {
				return typedValue.value;
			}
			return defaultValue;
		}

		public TValue Get<TValue>(string keyName, TValue defaultValue = default) {
			return Get<TValue>(new SpawnDataKey(keyName), defaultValue);
		}

		public bool Contains(SpawnDataKey key) => this.ContainsKey(key);

		public bool Contains(string keyName) => this.ContainsKey(new SpawnDataKey(keyName));

		public SpawnDataValue<TValue> GetWrapped<TValue>(SpawnDataKey key, SpawnDataValue<TValue> defaultWrapped = default) {
			if (this.TryGetValue(key, out var wrappedValue) && wrappedValue is SpawnDataValue<TValue> typedValue) {
				return typedValue;
			}
			return defaultWrapped;
		}

		public void Set<TValue>(SpawnDataKey key, SpawnDataValue<TValue> value) => this.Add(key, value);

		public void Set<TValue>(SpawnDataKey key, TValue value) {
			this.Set<TValue>(key, new SpawnDataValue<TValue>(value));
		}

		public void Set<TValue>(string keyName, TValue value) {
			this.Set<TValue>(new SpawnDataKey(keyName), new SpawnDataValue<TValue>(value));
		}

		public void Set(SpawnDataKey key, SpawnDataValue value) => this.Add(key, value);
	}
}
