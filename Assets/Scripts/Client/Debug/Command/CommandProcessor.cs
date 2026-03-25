using SoulboundBackend.Client.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.Debug.Commands {
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
			if (!tokens.Any()) return;
			Validate(input);

			CommandArguments args = new();
			CommandParsingContext ctx = new(args, dataProvider, execServices);

			CommandNode currentNode = rootNode;
			for (int i = 0; i < tokens.Length; i++) {
				currentNode = currentNode
					.GetChildren()
					.FirstOrDefault(n => n.Matches(tokens[i], ctx))
					?? throw new UnexpectedCommandException(tokens, i);
			}

			currentNode.GetHandler()(ctx);
		}

		public IEnumerable<CommandCompletionToken> GetCompletions(string input, int caretPos) {
			string[] tokens = Tokenize(input);
			CommandArguments args = new();
			CommandParsingContext ctx = new(args, dataProvider, execServices);

			if (!tokens.Any() || caretPos == 0) yield break;
			if (caretPos > input.Length) yield break;

			for (int t = 0; t < tokens.Length - 1; t++) {
				if (string.IsNullOrWhiteSpace(tokens[t])) yield break;
			}

			CommandNode currentNode = rootNode;
			int lastFullMatchIndex = 0;
			int lastTokenSpan = 1;	// includes the leading '/'
			for (lastFullMatchIndex = 0; lastFullMatchIndex < tokens.Length; lastFullMatchIndex++) {
				List<CommandNode> fullMatch = currentNode
					.GetChildren()
					.Where(c => c.Matches(tokens[lastFullMatchIndex], ctx))
					.ToList();

				lastTokenSpan += tokens[lastFullMatchIndex].Length + 1;

				if (!fullMatch.Any()) break;
				if (lastTokenSpan - 1 >= caretPos) break;

				currentNode = fullMatch.First();
			}
			lastTokenSpan--;    // remove last whitespace

			string lastToken = tokens[lastFullMatchIndex];

			int caretRelativeRemaining = lastTokenSpan - caretPos;
			if (caretRelativeRemaining < 0) yield break;

			int caretRelativePos = lastToken.Length - caretRelativeRemaining;
			string caretRelativeToken = lastToken[..caretRelativePos];

			foreach (var child in currentNode.GetChildren()) {
				foreach (var completion in child.GetCompletions(caretRelativeToken, ctx)) {
					yield return new CommandCompletionToken {
						text = completion,
						replaceLength = lastToken.Length,
						absoluteStart = lastTokenSpan - lastToken.Length,
					};
				}
			}
		}

		private void Validate(string input) {
			string[] tokens = Tokenize(input);
			CommandArguments args = new();
			CommandParsingContext ctx = new(args, dataProvider, execServices);

			for (int t = 0; t < tokens.Length; t++) {
				if (string.IsNullOrWhiteSpace(tokens[t])) {
					throw new InvalidCommandSyntaxException("Unexpected white space", tokens, t, format: "{0}<<");
				}
			}

			CommandNode currentNode = rootNode;
			int i;
			for (i = 0; i < tokens.Length; i++) {
				List<CommandNode> matchingNodes = new();
				foreach (var child in currentNode.GetChildren()) {
					if (child.Matches(tokens[i], ctx)) {
						matchingNodes.Add(child);
					}
				}
				if (matchingNodes.Count > 1) throw new AmbiguousCommandException(
					matchingNodes.Select(n => n.label).ToArray(), tokens, i
				);
				if (!matchingNodes.Any()) {
					throw new UnknownOrIncompleteCommandException(tokens, i);
				}
				currentNode = matchingNodes.First();
			}

			if (!currentNode.IsTerminalNode()) {
				throw new CommandNodeNotTerminalException(tokens, tokens.Length - 1);
			}
		}

		private string[] Tokenize(string input) {
			if (string.IsNullOrEmpty(input)) return Array.Empty<string>();
			if (!input.StartsWith("/")) return Array.Empty<string>();

			input = input[1..];
			return input.Split(' ');
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
			RebuildRoot();
		}
		public void UnregisterProvider(ICommandProvider provider) {
			providerBuffer.Remove(provider);
			RebuildRoot();
		}

		private void RebuildRoot() {
			CommandBuilder currentNodeBuilder = CommandBuilder.Literal("_root_");
			foreach (var command in EnumerateAllCommands()) {
				currentNodeBuilder.Then(command);
			}
			rootNode = currentNodeBuilder.GetRootNode();
		}
	}
}
