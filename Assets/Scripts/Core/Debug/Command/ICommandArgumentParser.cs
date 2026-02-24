using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public interface ICommandArgumentParser<T> {
		bool TryParse(string token, out T value);
	}
}
