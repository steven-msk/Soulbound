using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Core {
	public interface IStringCodec<T> {
		string Encode(T? value);
		T? Decode(string value);
	}
}
