using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Client.Debug.Commands {
	public class LiteralCommandNode : CommandNode {
		public override string label { get; }

		public LiteralCommandNode(string label, CommandHandler? handler = null)
			: base(handler) {
			this.label = label;
		} 

		public override bool Matches(string token, RuntimeCommandSource ctx) {
			return token == label;
		}

		public override IEnumerable<string> GetCompletions(string partialToken, RuntimeCommandSource ctx) {
			if (label.StartsWith(partialToken)) {
				yield return label;
			}
			yield break;
		}
	}
}
