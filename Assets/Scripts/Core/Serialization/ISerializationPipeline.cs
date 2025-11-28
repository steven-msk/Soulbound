using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Serialization {
	public interface ISerializationPipeline<T> {
		byte[] Write(T obj);
		T Read(byte[] data);
	}
}
