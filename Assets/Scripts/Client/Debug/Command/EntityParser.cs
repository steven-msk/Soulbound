using SoulboundBackend.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public sealed class EntityParser : ICommandArgumentParser<Guid> {
		public ParseResult<Guid> TryParse(string token, CommandParsingContext ctx) {
			if (!Guid.TryParse(token, out Guid guid)) {
				return ParseResult<Guid>.Fail();
			}

			return ctx.Data.Entities.TryGetEntity(guid, out IEntityView entity)
				? ParseResult<Guid>.Success(entity.GetGuid())
				: ParseResult<Guid>.Fail();
		}
	}
}
