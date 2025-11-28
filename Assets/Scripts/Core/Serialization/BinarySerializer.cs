using SoulboundBackend.Client.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Serialization {
	public class BinarySerializer<T> : ISerializer<T> {
		private readonly Func<BinaryReader, T> deserializer;
		private readonly Action<BinaryWriter, T> serializer;

		public BinarySerializer(Func<BinaryReader, T> deserializer, Action<BinaryWriter, T> serializer) {
			this.deserializer = deserializer;
			this.serializer = serializer;
		}

		public virtual T Deserialize(byte[] data) {
			using var memoryStream = new MemoryStream(data);
			using var reader = new BinaryReader(memoryStream);

			return deserializer.Invoke(reader);
		}

		public virtual byte[] Serialize(T obj) {
			using var memoryStream = new MemoryStream();
			using var writer = new BinaryWriter(memoryStream);

			serializer.Invoke(writer, obj);

			return memoryStream.ToArray();
		}
	}
}
