using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public class ItemParser : ICommandArgumentParser<Item> {
		public ParseResult<Item> TryParse(string token, CommandParsingContext ctx) {
			return Registry<Item>.TryGet(new Item.RegistrationKey(token), out Item item)
				? ParseResult<Item>.Success(item)
				: ParseResult<Item>.Fail();
		}
	}
}
