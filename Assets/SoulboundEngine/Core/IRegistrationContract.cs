using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Core {
	public interface IRegistrationContract<T, out TKey> where TKey : IRegistrationKey<T> {
		TKey ValueToKey(T value);
	}
}
