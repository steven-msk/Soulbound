using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Core;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Debug.Commands {
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
