using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public class ItemArgumentCommandNode : ArgumentCommandNode<Item> {
		public ItemArgumentCommandNode(string label, CommandHandler handler = null)
			: base(label, new ItemParser(), handler) {
		}

		public override IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext ctx) {
			Item[] items = ItemRegistry.GetAll().ToArray();
			foreach (var item in items) {
				if (item.GetID().StartsWith(partialToken)) {
					yield return item.GetID();
				}
			}
		}
	}
}
