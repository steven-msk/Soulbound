using SoulboundBackend.Client;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

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
			if (!Validate(input)) return;

			string[] tokens = Tokenize(input);
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

		public IEnumerable<CommandCompletionToken> GetCompletions(string input) {
			string[] tokens = Tokenize(input);
			CommandArguments args = new();
			CommandParsingContext ctx = new(args, dataProvider, execServices);

			if (tokens == null || tokens.Length == 0) yield break;

			for (int t = 0; t < tokens.Length - 1; t++) {
				if (string.IsNullOrWhiteSpace(tokens[t])) yield break;
			}

			CommandNode previousNode = rootNode;
			CommandNode currentNode = rootNode;
			int i;
			for (i = 0; i < tokens.Length; i++) {
				List<CommandNode> fullMatch = currentNode
					.GetChildren()
					.Where(c => c.Matches(tokens[i], ctx))
					.ToList();
				if (fullMatch.Count > 1) break;
				if (!fullMatch.Any()) break;
				previousNode = currentNode;
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

			foreach (var child in previousNode.GetChildren()) {
				if (child.Matches(tokens.Last(), ctx)) continue;

				foreach (var completion in child.GetCompletions(tokens.Last(), ctx)) {
					yield return completion;
				}
			}
		}

		private bool Validate(string input) {
			string[] tokens = Tokenize(input);
			Logger.LogInfo(string.Join(',', tokens.Select(s => string.IsNullOrEmpty(s) ? "EMPTY" : s)));

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
				throw new UnknownOrIncompleteCommandException(tokens, tokens.Length - 1);
			}

			return true;
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

	public static class CommandFormat {
		public const string MARKER_FORMAT = ">>{0}<<";

		public static string FormatWhere(string[] tokens, int tokenIndex, string format = MARKER_FORMAT) {
			StringBuilder builder = new();

			int startPrefix = Mathf.Max(0, tokenIndex - 3);
			builder.AppendJoin(' ', tokens[startPrefix..tokenIndex]);

			builder.Append(' ').AppendFormat(format, tokens[tokenIndex]);

			int endSuffix = Mathf.Min(tokens.Length, tokenIndex + 3);
			builder.AppendJoin(' ', tokens[(tokenIndex + 1)..endSuffix]);

			return builder.ToString();
		}

		public static string FormatQuoting(string[] entries) {
			return string.Join(", ", entries.Select(s => $"'{s}'"));
		}
	}

	public sealed class InvalidCommandSyntaxException : Exception {
		public InvalidCommandSyntaxException(string message, string[] tokens, int tokenIndex, string format = CommandFormat.MARKER_FORMAT)
			: base($"{message}: {CommandFormat.FormatWhere(tokens, tokenIndex, format)}") {
		}

		public InvalidCommandSyntaxException(string message)
			: base(message) {
		}
	}

	public sealed class AmbiguousCommandException : Exception {
		public AmbiguousCommandException(string[] matching, string[] tokens, int tokenIndex, string format = CommandFormat.MARKER_FORMAT)
			:  base($"Ambiguity between {CommandFormat.FormatQuoting(matching)}" +
				   $" at token '{tokens[tokenIndex]}': " +
				   $"\"{CommandFormat.FormatWhere(tokens, tokenIndex, format)}\"") {
		}
	}

	public sealed class UnknownOrIncompleteCommandException : Exception {
		public UnknownOrIncompleteCommandException(string[] tokens, int tokenIndex)
			: base($"Unknown or incomplete command at token '{tokens[tokenIndex]}': " +
				  $"{CommandFormat.FormatWhere(tokens, tokenIndex)}") {
		}
	}

	public sealed class UnexpectedCommandException : Exception {
		public UnexpectedCommandException(string[] tokens, int tokenIndex)
			: base($"Unexpected command exception at token '{tokens[tokenIndex]}': " +
				  $"{CommandFormat.FormatWhere(tokens, tokenIndex)}") {
		}
	}
}
