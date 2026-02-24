using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public class LiteralCommandNode : CommandNode {
		public override string label { get; }

		public LiteralCommandNode(string label, bool isExecutable = false)
			: base(isExecutable) {
			this.label = label;
		} 

		public override bool Matches(string token, CommandContext context) {
			return token == label;
		}
	}
}
