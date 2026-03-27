using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class EntityInstanceCompletionSupplier : ICommandCompletionSupplier {
		public IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext context) {
			foreach (var entity in context.Data.Entities.GetAllEntities().ToList()) {
				string value = entity.GetGuid().ToString();
				if (value.StartsWith(partialToken) || entity.GetID().StartsWith(partialToken)) {
					yield return value;
				}
			}
		}
	}
}
