using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public class EntityTypeArgumentCommandNode : ArgumentCommandNode<EntityDescriptor> {
		public EntityTypeArgumentCommandNode(string label, CommandHandler handler = null)
			: base(label, new EntityDescriptorParser(), handler) {
		}

		public override IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext ctx) {
			foreach (var entityDescriptor in Registry<EntityDescriptor>.GetAll()) {
				if (entityDescriptor.GetID().StartsWith(partialToken)) {
					yield return entityDescriptor.GetID();
				}
			}
		}
	}
}
