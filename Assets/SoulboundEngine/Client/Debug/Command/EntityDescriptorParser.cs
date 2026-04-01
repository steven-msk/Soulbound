using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class EntityDescriptorParser : ICommandArgumentParser<EntityDescriptor> {
		public ParseResult<EntityDescriptor> TryParse(string token, CommandParsingContext ctx) {
			if (!Identifier.TryFromString(token, out var identifier)) {
				return ParseResult<EntityDescriptor>.Fail();
			}

			return Registry<EntityDescriptor>.TryGet(identifier, out EntityDescriptor entityDescriptor)
				? ParseResult<EntityDescriptor>.Success(entityDescriptor)
				: ParseResult<EntityDescriptor>.Fail();
		}
	}
}
