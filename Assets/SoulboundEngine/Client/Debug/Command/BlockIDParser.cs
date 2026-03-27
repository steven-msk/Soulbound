using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class BlockIDParser : ICommandArgumentParser<Block> {
		public ParseResult<Block> TryParse(string token, CommandParsingContext ctx) {
			return Registry<Block>.TryGet(new Block.RegistrationKey(token), out Block block)
				? ParseResult<Block>.Success(block)
				: ParseResult<Block>.Fail();
		}
	}
}
