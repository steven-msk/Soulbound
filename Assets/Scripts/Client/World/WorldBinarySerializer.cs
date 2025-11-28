using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World {
	public sealed class WorldBinarySerializer : BinarySerializer<WorldDump> {
		private static readonly Func<BinaryReader, WorldDump> deserializer = reader => {
			throw new NotImplementedException();
		};

		private static readonly Action<BinaryWriter, WorldDump> serializer = (writer, obj) => {
			writer.Write(obj.seed);
			writer.WriteArray(obj.generatedChunks, WorldChunk.Serializer.WriteBinary);
			throw new NotImplementedException();
		};

		public WorldBinarySerializer() : base(deserializer, serializer) {
		}
	}
}
