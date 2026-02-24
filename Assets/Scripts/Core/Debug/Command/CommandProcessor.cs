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
			ArgumentCommandNode<Coordinate> x = new("x", new CoordinateParser());
			ArgumentCommandNode<Coordinate> y = new("y", new CoordinateParser(), true);
			x.AddChild(y);
			tp.AddChild(x);
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

			if (currentNode.IsExecutable()) {
				Logger.LogInfo("completed command: {}", input);
			} else {
				Logger.LogInfo("incorrect command: {}", input);
			}

			//ProcessChildrenRecursive(root, tokens, 1, context);
		}

		public void ProcessChildrenRecursive(CommandNode root, string[] tokens, int tokenIndex, CommandContext context) {
			if (tokenIndex >= tokens.Length) {
				Logger.LogInfo("reached end of children");
				return;
			}

			List<CommandNode> matching = new();

			foreach (var child in root.GetChildren()) {
				if (child.HasChildren()) ProcessChildrenRecursive(child, tokens, tokenIndex + 1, context);

				if (child.Matches(tokens[tokenIndex], context)) matching.Add(child);
			}

			if (matching.Count > 1) {
				Logger.LogInfo("ambiguity between children {}", tokens[tokenIndex]);
				return;
			}

			if (!matching.Any() && root.HasChildren()) {
				Logger.LogInfo("no matching child {}", tokens[tokenIndex]);
				return;
			}

			Logger.LogInfo("completed command: /{}", string.Join(" ", tokens));
		}

		private string[] Tokenize(string input) {
			return input[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
		}

	}
}
