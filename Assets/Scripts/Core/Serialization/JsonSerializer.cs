using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoulboundBackend.Core.Serialization {
	public class JsonSerializer<T> : ISerializer<T> {
		private JsonSerializerSettings serializerSettings;

		public JsonSerializer(JsonSerializerSettings serializerSettings) {
			this.serializerSettings = serializerSettings;
		}
		public byte[] Serialize(T obj) {
			string json = JsonConvert.SerializeObject(obj, serializerSettings);
			return Encoding.UTF8.GetBytes(json);
		}

		public T Deserialize(byte[] data) {
			string json = Encoding.UTF8.GetString(data);
			return JsonConvert.DeserializeObject<T>(json, serializerSettings);
		}
	}
}
