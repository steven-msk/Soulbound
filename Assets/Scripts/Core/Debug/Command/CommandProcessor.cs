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
			CommandBuilder command = CommandBuilder.Literal("command");
			command.Then(new LiteralCommandNode("subcommand1"))
				.Then(new LiteralCommandNode("anotherSubCommand"))
				.Executes(args => Logger.LogInfo("executing anotherSubCommand"));
			command.Then(new LiteralCommandNode("subcommand"))
				.Executes(args => Logger.LogInfo("subcommand2"));
			command.Then(new LiteralCommandNode("subcommand3"))
				.Executes(args => Logger.LogInfo("impossible to run"));
			command.Then(new LiteralCommandNode("subcommand3"))
				.Executes(args => Logger.LogInfo("impossible to run"));
			registry.Add(command.GetRootNode());

			CommandBuilder coords = CommandBuilder.Create(new ArgumentCommandNode<Coordinate>("x", new CoordinateParser()))
				.Then(new ArgumentCommandNode<Coordinate>("y", new CoordinateParser()))
				.Executes(args => Logger.LogInfo("teleported"));

			CommandBuilder tp = CommandBuilder.Literal("tp");
			tp.Then(coords);
			tp.Then(new ArgumentCommandNode<Guid>("target", new GuidParser()))
				.Then(coords);
			registry.Add(tp.GetRootNode());
		}

		public void Process(string input) {
			string[] tokens = Tokenize(input);
			CommandArguments args = new();

			if (tokens == null || tokens.Length == 0) {
				Logger.LogInfo("empty command");
				return;
			}

			List<CommandNode> matching = registry
				.Where(n => n.Matches(tokens[0], args))
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
					.Where(n => n.Matches(tokens[i], args))
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
