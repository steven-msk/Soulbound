using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;

namespace SoulboundBackend.Core.Serialization {
	[PROTOTYPICAL]
	public sealed class SerializationPipeline<T> : ISerializationPipeline<T> {
		private readonly ISerializer<T> serializer;

		public SerializationPipeline(ISerializer<T> serializer) {
			this.serializer = serializer;
		}

		public T Read(byte[] data) {
			return serializer.Deserialize(data);
		}

		public byte[] Write(T obj) {
			return serializer.Serialize(obj);
		}
	}
}
