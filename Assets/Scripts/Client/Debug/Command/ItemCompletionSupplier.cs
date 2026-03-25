using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using System.Collections.Generic;

namespace SoulboundBackend.Client.Debug.Commands {
	public sealed class ItemCompletionSupplier : ICommandCompletionSupplier {
		public IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext context) {
			foreach (var item in Registry<Item>.GetAll()) {
				if (item.GetID().StartsWith(partialToken)) {
					yield return item.GetID();
				}
			}
		}
	}
}
