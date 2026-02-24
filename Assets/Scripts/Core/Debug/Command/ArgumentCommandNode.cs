using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public class ArgumentCommandNode<T> : CommandNode {
		private readonly ICommandArgumentParser<T> parser;
		public override string label { get; }

		public ArgumentCommandNode(string label, ICommandArgumentParser<T> parser, bool isTerminal = false)
			: base(isTerminal) {
			this.label = label;
			this.parser = parser;
		}

		public override bool Matches(string token, CommandContext context) {
			if (!parser.TryParse(token, out T value)) return false;

			context.SetArgument(token, value);
			return true;
		}
	}
}
