using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Core.Serialization {
	public interface ISerializer<T> {
		byte[] Serialize(T obj);
		T Deserialize(byte[] data);
	}
}
