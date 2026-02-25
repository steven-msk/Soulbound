using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Core.Debug.Commands {
	public class LiteralCommandNode : CommandNode {
		public override string label { get; }

		public LiteralCommandNode(string label, CommandHandler? handler = null)
			: base(handler) {
			this.label = label;
		} 

		public override bool Matches(string token, CommandParsingContext ctx) {
			return token == label;
		}
	}
}
