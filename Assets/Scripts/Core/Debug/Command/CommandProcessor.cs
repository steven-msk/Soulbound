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
		private CommandNode rootNode;

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

			CommandNode currentNode = rootNode;
			if (currentNode == null) {
				Logger.LogInfo("no matching command {}", tokens[0]);
				return;
			}

			for (int i = 0; i < tokens.Length; i++) {
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

		public IEnumerable<CommandCompletionToken> GetCompletions(string input) {
			string[] tokens = Tokenize(input);
			Logger.LogInfo(string.Join(", ", tokens));
			CommandArguments args = new();
			CommandParsingContext ctx = new(args, dataProvider, execServices);

			if (tokens == null || tokens.Length == 0) yield break;

			CommandNode currentNode = rootNode;
			int i;
			for (i = 0; i < tokens.Length; i++) {
				List<CommandNode> fullMatch = currentNode
					.GetChildren()
					.Where(c => c.Matches(tokens[i], ctx))
					.ToList();
				if (fullMatch.Count > 1) break;
				if (!fullMatch.Any()) break;
				currentNode = fullMatch.First();
			}

			if (i < tokens.Length) {
				string partialToken = tokens[i];
				foreach (var child in currentNode.GetChildren()) {
					foreach (var completion in child.GetCompletions(partialToken, ctx)) {
						yield return completion;
					}
				}
				yield break;
			}

			foreach (var child in currentNode.GetChildren()) {
				foreach (var completion in child.GetCompletions("", ctx)) {
					yield return completion;
				}
			}
		}

		private string[] Tokenize(string input) {
			if (string.IsNullOrEmpty(input)) return Array.Empty<string>();
			if (!input.StartsWith("/")) return Array.Empty<string>();

			input = input[1..];

			List<string> tokens = input
				.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.ToList();
			return tokens.ToArray();
		}

		private IEnumerable<CommandNode> EnumerateAllCommands() {
			for (int i = 0; i < providerBuffer.Count; i++) {
				foreach (var command in providerBuffer[i].GetCommands()) {
					yield return command;
				}
			}
		}

		private void RebuildRoot() {
			CommandBuilder currentNodeBuilder = CommandBuilder.Literal("_root_");
			foreach (var command in EnumerateAllCommands()) {
				currentNodeBuilder.Then(command);
			}
			rootNode = currentNodeBuilder.GetRootNode();
		}

		public void RegisterProvider(ICommandProvider provider) {
			providerBuffer.Add(provider);
			RebuildRoot();
		}
		public void UnregisterProvider(ICommandProvider provider) {
			providerBuffer.Remove(provider);
			RebuildRoot();
		}

	}
}
