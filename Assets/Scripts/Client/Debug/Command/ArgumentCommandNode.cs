using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.Debug.Commands {
	public class ArgumentCommandNode<T> : CommandNode {
		private readonly ICommandArgumentParser<T> parser;
		private readonly ICommandCompletionSupplier? completionSupplier;
		public override string label { get; }

		public ArgumentCommandNode(string label, ICommandArgumentParser<T> parser, ICommandCompletionSupplier? completionSupplier = null, CommandHandler? handler = null)
			: base(handler) {
			this.label = label;
			this.parser = parser;
			this.completionSupplier = completionSupplier;
		}

		public override bool Matches(string token, CommandParsingContext ctx) {
			ParseResult<T> result = parser.TryParse(token, ctx);
			if (!result.success) return false;

			ctx.Args.Set(label, result.value);
			return true;
		}

		public override IEnumerable<string> GetCompletions(string partialToken, CommandParsingContext ctx) {
			if (completionSupplier == null) yield break;

			foreach (var completion in completionSupplier.GetCompletions(partialToken, ctx)) {
				yield return completion;
			}
		}
	}
}
