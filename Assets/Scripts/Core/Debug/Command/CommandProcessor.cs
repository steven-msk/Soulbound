using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandProcessor {
		private readonly List<CommandNode> registry = new();

		public CommandProcessor() {
			CommandNode node = new LiteralCommandNode("command");

			CommandNode subcommand = new LiteralCommandNode("subcommand1");
			subcommand.AddChild(new LiteralCommandNode("anotherSubCommand", true));

			node.AddChild(subcommand);
			node.AddChild(new LiteralCommandNode("subcommand2", true));
			node.AddChild(new LiteralCommandNode("subcommand3", true));
			node.AddChild(new LiteralCommandNode("subcommand3", true));
			registry.Add(node);

			CommandNode tp = new LiteralCommandNode("tp");
			ArgumentCommandNode<Guid> target = new("target", new GuidParser());
			ArgumentCommandNode<Coordinate> x = new("x", new CoordinateParser());
			ArgumentCommandNode<Coordinate> y = new("y", new CoordinateParser(), true);
			x.AddChild(y);
			target.AddChild(x);
			tp.AddChild(x);
			tp.AddChild(target);
			registry.Add(tp);
		}

		public void Process(string input) {
			string[] tokens = Tokenize(input);
			CommandContext context = new();

			if (tokens == null || tokens.Length == 0) {
				Logger.LogInfo("empty command");
				return;
			}

			List<CommandNode> matching = registry
				.Where(n => n.Matches(tokens[0], context))
				.ToList();
			if (matching.Count > 1) {
				Logger.LogInfo("ambiguity between commands {}", tokens[0]);
				return;
			}

			CommandNode currentNode = matching.FirstOrDefault();
			if (currentNode == null) {
				Logger.LogInfo("no matching command {}", tokens[0]);
				return;
			}

			for (int i = 1; i < tokens.Length; i++) {
				List<CommandNode> matchingSubnodes = currentNode
					.GetChildren()
					.Where(n => n.Matches(tokens[i], context))
					.ToList();
				if (matchingSubnodes.Count > 1) {
					Logger.LogInfo("ambiguity between commands {}", tokens[i]);
					return;
				}

				if (!matchingSubnodes.Any()) {
					Logger.LogInfo("no matching command {}", tokens[i]);
					return;
				}
				currentNode = matchingSubnodes.FirstOrDefault();
			}

			if (currentNode.IsTerminalNode()) {
				Logger.LogInfo("completed command: {}", input);
			} else {
				Logger.LogInfo("incorrect command: {}", input);
			}

		}

		private string[] Tokenize(string input) {
			return input[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
		}

	}
}
