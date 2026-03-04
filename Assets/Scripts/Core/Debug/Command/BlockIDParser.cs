using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class BlockIDParser : ICommandArgumentParser<Block> {
		public ParseResult<Block> TryParse(string token, CommandParsingContext ctx) {
			return BlockRegistry.TryGet(token, out Block block)
				? ParseResult<Block>.Success(block)
				: ParseResult<Block>.Fail();
		}
	}
}
