using SoulboundBackend.Core.Debug.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public class GuidParser : ICommandArgumentParser<Guid> {
		public bool TryParse(string token, out Guid value) {
			return Guid.TryParse(token, out value);
		}
	}
}
