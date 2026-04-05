using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.Debug.Commands {
	[Obsolete]
	public delegate void CommandHandler(RuntimeCommandSource ctx);

	[Obsolete]
	public abstract class CommandNode {
		public abstract string label { get; }
		protected readonly List<CommandNode> children = new();
		protected CommandHandler? handler;

		protected CommandNode(CommandHandler? handler = null) {
			this.handler = handler;
		}

		public abstract bool Matches(string token, RuntimeCommandSource ctx);
		public abstract IEnumerable<string> GetCompletions(string partialToken, RuntimeCommandSource ctx);

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
