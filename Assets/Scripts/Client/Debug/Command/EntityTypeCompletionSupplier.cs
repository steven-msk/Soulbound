using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Core;
using System.Collections.Generic;

namespace SoulboundBackend.Client.Debug.Commands {
	public sealed class EntityTypeCompletionSupplier : ICommandCompletionSupplier {
		public IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext context) {
			foreach (var entityDescriptor in Registry<EntityDescriptor>.GetAll()) {
				if (entityDescriptor.GetID().StartsWith(partialToken)) {
					yield return entityDescriptor.GetID();
				}
			}
		}
	}
}
