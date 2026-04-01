using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class EntityTypeCompletionSupplier : ICommandCompletionSupplier {
		public IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext context) {
			foreach (var entityDescriptor in Registry<EntityDescriptor>.GetAll()) {
				Identifier identifier = entityDescriptor.GetIdentifier();

				if (identifier.IsPartiallyMatching(partialToken)) {
					yield return identifier.ToString();
				}
			}
		}
	}
}
