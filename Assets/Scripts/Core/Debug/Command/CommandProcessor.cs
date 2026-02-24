using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandProcessor {
		private readonly ICommandProvider commandProvider;

		public CommandProcessor(ICommandProvider commandProvider) {
			this.commandProvider = commandProvider;
		}

		public void Process(string input) {
			string[] tokens = Tokenize(input);
			CommandArguments args = new();

			if (tokens == null || tokens.Length == 0) {
				Logger.LogInfo("empty command");
				return;
			}

			string rootToken = tokens[0];
			List<CommandNode> matching = new();
			foreach (var command in commandProvider.GetCommands()) {
				if (command.Matches(rootToken, args)) {
					matching.Add(command);

					if (matching.Count > 1) {
						Logger.LogInfo("ambiguity between commands {}", tokens[0]);
						return;
					}
				}
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
