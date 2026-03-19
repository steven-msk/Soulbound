using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public sealed class StringParser : ICommandArgumentParser<string> {
		public ParseResult<string> TryParse(string token, CommandParsingContext ctx) {
			return ParseResult<string>.Success(token);
		}
	}
}
