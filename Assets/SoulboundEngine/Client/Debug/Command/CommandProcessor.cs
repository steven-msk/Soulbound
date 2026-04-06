using Brigadier.NET;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed record RuntimeCommandSource(
		IRuntimeDataProvider data,
		IRuntimeExecutionServices execServices
	);

	public sealed class CommandProcessor {
		private readonly List<ICommandProvider> providerBuffer = new();
		private readonly IRuntimeDataProvider dataProvider;
		private readonly IRuntimeExecutionServices execServices;
		private CommandDispatcher<RuntimeCommandSource> dispatcher = new();

		public CommandProcessor(IRuntimeDataProvider dataProvider, IRuntimeExecutionServices execServices) {
			this.dataProvider = dataProvider;
			this.execServices = execServices;
		}

		public void SubmitCommand(string input) {
			if (input.StartsWith('/')) input = input[1..];

			RuntimeCommandSource source = new(dataProvider, execServices);
			ParseResults<RuntimeCommandSource> parseResults = dispatcher.Parse(input, source);

			int code = 0;
			try {
				if (parseResults.Exceptions.Any()) {
					throw parseResults.Exceptions.First().Value;
				}
				code = dispatcher.Execute(parseResults);
			} catch (Exception e) when (e is CommandSyntaxException) {
				Logger.LogFatal(e);
				code = -1;
			} finally {
				Logger.LogInfo("Command dispatched with exit code {}", code);
			}
		}

		public async UniTask<Suggestions> GetCompletions(string input, int caretPos) {
			if (input.StartsWith('/')) {
				input = input[1..];
				caretPos--;
			}
			RuntimeCommandSource source = new(dataProvider, execServices);
			Task<Suggestions> task;
			ParseResults<RuntimeCommandSource> parseResults = dispatcher.Parse(input, source);

			try {
				task = dispatcher.GetCompletionSuggestions(parseResults, caretPos);
			} catch (CommandSyntaxException e) {
				Logger.LogError(e);
				task = Suggestions.Empty();
			}
			await task;

			return task.Result;
		}

		[Obsolete]
		private void Validate(string input) {
			//string[] tokens = Tokenize(input);
			//CommandArguments args = new();
			//CommandParsingContext ctx = new(args, dataProvider, execServices);

			//for (int t = 0; t < tokens.Length; t++) {
			//	if (string.IsNullOrWhiteSpace(tokens[t])) {
			//		throw new InvalidCommandSyntaxException("Unexpected white space", tokens, t, format: "{0}<<");
			//	}
			//}

			//CommandNode currentNode = rootNode;
			//int i;
			//for (i = 0; i < tokens.Length; i++) {
			//	List<CommandNode> matchingNodes = new();
			//	foreach (var child in currentNode.GetChildren()) {
			//		if (child.Matches(tokens[i], ctx)) {
			//			matchingNodes.Add(child);
			//		}
			//	}
			//	if (matchingNodes.Count > 1) throw new AmbiguousCommandException(
			//		matchingNodes.Select(n => n.label).ToArray(), tokens, i
			//	);
			//	if (!matchingNodes.Any()) {
			//		throw new UnknownOrIncompleteCommandException(tokens, i);
			//	}
			//	currentNode = matchingNodes.First();
			//}

			//if (!currentNode.IsTerminalNode()) {
			//	throw new CommandNodeNotTerminalException(tokens, tokens.Length - 1);
			//}
		}

		[Obsolete]
		private string[] Tokenize(string input) {
			return Array.Empty<string>();
			//if (string.IsNullOrEmpty(input)) return Array.Empty<string>();
			//if (!input.StartsWith("/")) return Array.Empty<string>();

			//input = input[1..];
			//return input.Split(' ');
		}

		[Obsolete]
		private IEnumerable<CommandNode> EnumerateAllCommands() {
			yield break;
			//for (int i = 0; i < providerBuffer.Count; i++) {
			//	foreach (var command in providerBuffer[i].GetCommands()) {
			//		yield return command;
			//	}
			//}
		}

		public void RegisterProvider(ICommandProvider provider) {
			providerBuffer.Add(provider);
			RebuildDispatcher();
		}
		public void UnregisterProvider(ICommandProvider provider) {
			providerBuffer.Remove(provider);
			RebuildDispatcher();
		}

		private void RebuildDispatcher() {
			dispatcher = new CommandDispatcher<RuntimeCommandSource>();
			foreach (var provider in providerBuffer) {
				provider.RegisterCommands(dispatcher);
			}
			dispatcher.FindAmbiguities((parent, child, sibling, inputs) => {
				Logger.LogFatal(new AmbiguousCommandException(
					parent.UsageText, child.UsageText, sibling.UsageText, inputs
				));
			});
		}
	}
}
