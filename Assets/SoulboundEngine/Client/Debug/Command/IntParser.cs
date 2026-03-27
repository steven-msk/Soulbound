using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class IntParser : ICommandArgumentParser<int> {
		public ParseResult<int> TryParse(string token, CommandParsingContext ctx) {
			return int.TryParse(token, out int value)
				? ParseResult<int>.Success(value)
				: ParseResult<int>.Fail();
		}
	}
}
