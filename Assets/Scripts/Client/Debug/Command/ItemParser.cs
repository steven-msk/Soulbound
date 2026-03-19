using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public class ItemParser : ICommandArgumentParser<Item> {
		public ParseResult<Item> TryParse(string token, CommandParsingContext ctx) {
			return ItemRegistry.TryGet(token, out Item item)
				? ParseResult<Item>.Success(item)
				: ParseResult<Item>.Fail();
		}
	}
}
