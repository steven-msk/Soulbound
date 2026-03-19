using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Serialization {
	public sealed class WorldBinarySerializer : BinarySerializer<WorldDump> {
		// version?
		
		protected override WorldDump ReadBinary(BinaryReader reader) {
			throw new NotImplementedException();
		}

		protected override byte[] WriteBinary(WorldDump obj, BinaryWriter writer) {
			writer.Write(obj.seed);
			writer.WriteArray(obj.generatedChunks, WorldChunk.Serializer.WriteBinary);
			throw new NotImplementedException();
		}
	}
}
