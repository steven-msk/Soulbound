using SoulboundBackend.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public class EntityTypeArgumentCommandNode : ArgumentCommandNode<EntityDescriptor> {
		public EntityTypeArgumentCommandNode(string label, CommandHandler handler = null)
			: base(label, new EntityDescriptorParser(), handler) {
		}

		public override IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext ctx) {
			foreach (var entityDescriptor in EntityRegistry.GetAll()) {
				if (entityDescriptor.id.StartsWith(partialToken)) {
					yield return entityDescriptor.id;
				}
			}
		}
	}
}
