using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Profiling;

namespace SoulboundEngine.Client.Debug.Commands {
	public class CommandBuilder {
		private readonly CommandNode root;
		private readonly CommandNode cursor;

		public CommandBuilder(CommandNode node) {
			root = cursor = node;
		}

		private CommandBuilder(CommandNode cursor, CommandNode root) {
			this.cursor = cursor;
			this.root = root;
		}

		public CommandBuilder Then(CommandNode child) {
			cursor.AddChild(child);
			return new CommandBuilder(child, root);
		}

		public CommandBuilder Executes(CommandHandler handler) {
			cursor.SetHandler(handler);
			return this;
		}

		public CommandBuilder ThenRootOf(CommandBuilder builder) {
			CommandNode root = builder.GetRootNode();
			cursor.AddChild(root);
			return new CommandBuilder(root, this.root);
		}

		public CommandBuilder ThenCursorOf(CommandBuilder builder) {
			CommandNode cursor = builder.GetCursorNode();
			this.cursor.AddChild(builder.GetRootNode());
			return new CommandBuilder(cursor, root);
		}

		public CommandNode GetCursorNode() => cursor;
		public CommandNode GetRootNode() => root;

		public static CommandBuilder Literal(string literal) {
			return new CommandBuilder(new LiteralCommandNode(literal));
		}

		public static CommandBuilder Create(CommandNode root) {
			return new CommandBuilder(root);
		}
	}
}
