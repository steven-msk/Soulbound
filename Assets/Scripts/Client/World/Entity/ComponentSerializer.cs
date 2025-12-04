using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace SoulboundBackend.Client.World.EntitySystem {
	[Obsolete]
	public sealed class ComponentSerializer {
		private readonly Dictionary<string, object> data = new();
		public Dictionary<string, object> raw => data;

		public void Set<T>(string key, T value) {
			data[key] = value;
		}

		public T Get<T>(string key) {
			return (T)data[key];
		}

		public T TryGet<T>(string key, T fallback = default) {
			return data.TryGetValue(key, out var obj)
				? (T)obj
				: fallback;
		}

	}
}