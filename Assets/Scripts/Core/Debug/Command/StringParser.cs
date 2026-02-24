using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class StringParser : ICommandArgumentParser<string> {
		public bool TryParse(string token, out string value) {
			value = token;
			return true;
		}
	}
}
