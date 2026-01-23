using SoulboundBackend.Client.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Serialization {
	public abstract class BinarySerializer<T> : ISerializer<T> {
		protected abstract T ReadBinary(BinaryReader reader);
		protected abstract byte[] WriteBinary(T obj, BinaryWriter writer);

		public virtual T Deserialize(byte[] data) {
			using var memoryStream = new MemoryStream(data);
			using var reader = new BinaryReader(memoryStream);

			return ReadBinary(reader);
		}

		public virtual byte[] Serialize(T obj) {
			using var memoryStream = new MemoryStream();
			using var writer = new BinaryWriter(memoryStream);

			WriteBinary(obj, writer);

			return memoryStream.ToArray();
		}
	}
}
