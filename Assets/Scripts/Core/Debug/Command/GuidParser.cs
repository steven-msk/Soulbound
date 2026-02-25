using SoulboundBackend.Core.Debug.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public class GuidParser : ICommandArgumentParser<Guid> {
		public ParseResult<Guid> TryParse(string token, CommandParsingContext ctx) {
			return Guid.TryParse(token, out Guid guid)
				? ParseResult<Guid>.Success(guid)
				: ParseResult<Guid>.Fail();
		}
	}
}
