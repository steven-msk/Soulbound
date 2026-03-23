using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core {
	public interface IRegistrationContract<T, out TKey> where TKey : IRegistrationKey<T> {
		TKey ValueToKey(T value);
	}
}
