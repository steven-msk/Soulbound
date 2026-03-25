using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Core;
using System.Collections.Generic;

namespace SoulboundBackend.Client.Debug.Commands {
	public sealed class BlockCompletionSupplier : ICommandCompletionSupplier {
		public IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext context) {
			foreach (var block in Registry<Block>.GetAll()) {
				if (block.GetID().StartsWith(partialToken)) {
					yield return block.GetID();
				}
			}
		}
	}
}
