using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class ItemCompletionSupplier : ICommandCompletionSupplier {
		public IEnumerable<string> GetCompletions(string partialToken, RuntimeCommandSource context) {
			foreach (var item in Registry<Item>.GetAll()) {
				Identifier identifier = item.GetIdentifier();

				if (identifier.IsPartiallyMatching(partialToken)) {
					yield return identifier.ToString();
				}
			}
		}
	}
}
