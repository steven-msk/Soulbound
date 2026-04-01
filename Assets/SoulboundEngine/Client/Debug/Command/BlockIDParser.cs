using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class BlockIDParser : ICommandArgumentParser<Block> {
		public ParseResult<Block> TryParse(string token, CommandParsingContext ctx) {
			if (!Identifier.TryFromString(token, out var identifier)) {
				return ParseResult<Block>.Fail();
			}

			UnityEngine.Debug.Log(identifier);

			return Registry<Block>.TryGet(identifier, out Block block)
				? ParseResult<Block>.Success(block)
				: ParseResult<Block>.Fail();
		}
	}
}
