using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.Debug.Commands {
	public class ItemParser : ICommandArgumentParser<Item> {
		public ParseResult<Item> TryParse(string token, CommandParsingContext ctx) {
			if (!Identifier.TryFromString(token, out var identifier)) {
				return ParseResult<Item>.Fail();
			}

			return Registry<Item>.TryGet(identifier, out Item item)
				? ParseResult<Item>.Success(item)
				: ParseResult<Item>.Fail();
		}
	}
}
