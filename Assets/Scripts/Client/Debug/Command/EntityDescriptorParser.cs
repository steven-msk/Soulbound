using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public sealed class EntityDescriptorParser : ICommandArgumentParser<EntityDescriptor> {
		public ParseResult<EntityDescriptor> TryParse(string token, CommandParsingContext ctx) {
			return Registry<EntityDescriptor>.TryGet(new EntityDescriptor.RegistrationKey(token), out EntityDescriptor entityDescriptor)
				? ParseResult<EntityDescriptor>.Success(entityDescriptor)
				: ParseResult<EntityDescriptor>.Fail();
		}
	}
}
