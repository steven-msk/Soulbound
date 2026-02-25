using SoulboundBackend.Client;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandProcessor {
		private readonly List<ICommandProvider> providerBuffer = new();
		private readonly IRuntimeDataProvider dataProvider;
		private readonly IRuntimeExecutionServices execServices;

		public CommandProcessor(IRuntimeDataProvider dataProvider, IRuntimeExecutionServices execServices) {
			this.dataProvider = dataProvider;
			this.execServices = execServices;
		}

		// Follows Brigadier parsing architecture
		public void SubmitCommand(string input) {
			string[] tokens = Tokenize(input);
			CommandArguments args = new();
			CommandParsingContext ctx = new(args, dataProvider, execServices);

			if (tokens == null || tokens.Length == 0) {
				Logger.LogInfo("empty command");
				return;
			}

			string rootToken = tokens[0];
			List<CommandNode> matching = new();
			foreach (var command in EnumerateAllCommands()) {
				if (command.Matches(rootToken, ctx)) {
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
					.Where(n => n.Matches(tokens[i], ctx))
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
				currentNode.GetHandler()(ctx);
			} else {
				Logger.LogInfo("incorrect command: {}", input);
			}

		}

		private string[] Tokenize(string input) {
			return input[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
		}

		private IEnumerable<CommandNode> EnumerateAllCommands() {
			for (int i = 0; i < providerBuffer.Count; i++) {
				foreach (var command in providerBuffer[i].GetCommands()) {
					yield return command;
				}
			}
		}

		public void RegisterProvider(ICommandProvider provider) {
			providerBuffer.Add(provider);
		}
		public void UnregisterProvider(ICommandProvider provider) {
			providerBuffer.Remove(provider);
		}

	}
}
