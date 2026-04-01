using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class BlockCompletionSupplier : ICommandCompletionSupplier {
		public IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext context) {
			foreach (var block in Registry<Block>.GetAll()) {
				Identifier identifier = block.GetIdentifier();

				if (identifier.IsPartiallyMatching(partialToken)) {
					yield return identifier.ToString();
				}
			}
		}
	}
}
