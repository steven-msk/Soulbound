using System;

namespace SoulboundEngine.Client.Debug.Commands {
	public class GuidParser : ICommandArgumentParser<Guid> {
		public ParseResult<Guid> TryParse(string token, CommandParsingContext ctx) {
			return Guid.TryParse(token, out Guid guid)
				? ParseResult<Guid>.Success(guid)
				: ParseResult<Guid>.Fail();
		}
	}
}
