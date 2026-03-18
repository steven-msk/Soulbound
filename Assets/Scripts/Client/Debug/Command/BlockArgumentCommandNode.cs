using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public class BlockArgumentCommandNode : ArgumentCommandNode<Block> {
		public BlockArgumentCommandNode(string label, CommandHandler handler = null)
			: base(label, new BlockIDParser(), handler) {
		}

		public override IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext ctx) {
			foreach (var block in BlockRegistry.AllBlocks()) {
				if (block.id.StartsWith(partialToken)) {
					yield return block.id;
				}
			}
		}
	}
}
