using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Core.Debug.Commands {
	public class EntityInstanceArgumentCommandNode : ArgumentCommandNode<Guid> {
		public EntityInstanceArgumentCommandNode(string label, CommandHandler? handler = null)
			: base(label, new EntityParser(), handler) {
		}

		public override IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext ctx) {
			foreach (var entity in ctx.Data.Entities.GetAllEntities().ToList()) {
				string value = entity.GetGuid().ToString();
				if (value.StartsWith(partialToken) || entity.GetName().StartsWith(partialToken)) {
					yield return value;
				}
			}
		}
	}
}
