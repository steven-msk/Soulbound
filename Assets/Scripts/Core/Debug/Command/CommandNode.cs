using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Core.Debug.Commands {
	public delegate void CommandHandler(CommandParsingContext ctx);

	public abstract class CommandNode {
		public abstract string label { get; }
		protected readonly List<CommandNode> children = new();
		protected CommandHandler? handler;

		protected CommandNode(CommandHandler? handler = null) {
			this.handler = handler;
		}

		public abstract bool Matches(string token, CommandParsingContext ctx);
		public abstract IEnumerable<CommandCompletionToken> GetCompletions(string partialToken, CommandParsingContext ctx);

		public void AddChild(CommandNode child) => children.Add(child);
		public IEnumerable<CommandNode> GetChildren() {
			for (int i = 0; i < children.Count; i++) {
				yield return children[i];
			}
		}

		public bool HasChildren() => children.Any();

		public bool IsTerminalNode() => handler != null;
		public CommandHandler? GetHandler() => handler;
		public void SetHandler(CommandHandler? handler) => this.handler = handler; 
	}
}
