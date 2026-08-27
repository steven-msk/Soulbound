namespace SoulboundEngine.UnityClient.Debug.Commands {
	using Brigadier.NET;
	using Brigadier.NET.Exceptions;
	using Brigadier.NET.Suggestion;
	using Brigadier.NET.Tree;
	using Cysharp.Threading.Tasks;
	using SoulboundEngine.World.Services;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

#nullable enable

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
			ParseResults<RuntimeCommandSource> parseResults = this.Parse(input);
			int code = 0;
			try {
				if (parseResults.Exceptions.Any()) {
					throw parseResults.Exceptions.First().Value;
				}
				code = this.dispatcher.Execute(parseResults);
			} catch (CommandSyntaxException e) {
				Logger.LogFatal(e);
				code = -1;
			} finally {
				Logger.LogInfo("Command dispatched with exit code {}", code);
			}
		}

		public IDictionary<CommandNode<RuntimeCommandSource>, string> GetSmartUsages(CommandNode<RuntimeCommandSource> node) {
			RuntimeCommandSource source = new(this.dataProvider, this.execServices);
			return this.dispatcher.GetSmartUsage(node, source);
		}

		public ParseResults<RuntimeCommandSource> Parse(string input) {
			if (input.StartsWith('/')) input = input[1..];
			RuntimeCommandSource source = new(this.dataProvider, this.execServices);
			return this.dispatcher.Parse(input, source);
		}

		public async UniTask<Suggestions> GetCompletions(string input, int caretPos) {
			if (input.StartsWith('/')) {
				input = input[1..];
				caretPos--;
			}
			ParseResults<RuntimeCommandSource> parseResults = this.Parse(input);
			Task<Suggestions> task;

			try {
				task = this.dispatcher.GetCompletionSuggestions(parseResults, caretPos);
			} catch (CommandSyntaxException e) {
				Logger.LogError(e);
				task = Suggestions.Empty();
			}
			return await task;
		}

		public void RegisterProvider(ICommandProvider provider) {
			this.providerBuffer.Add(provider);
			this.RebuildDispatcher();
		}
		public void UnregisterProvider(ICommandProvider provider) {
			this.providerBuffer.Remove(provider);
			this.RebuildDispatcher();
		}

		private void RebuildDispatcher() {
			this.dispatcher = new CommandDispatcher<RuntimeCommandSource>();
			foreach (ICommandProvider provider in this.providerBuffer) {
				provider.RegisterCommands(this.dispatcher);
			}
			this.dispatcher.FindAmbiguities((parent, child, sibling, inputs) => {
				Logger.LogFatal(new AmbiguousCommandException(
					parent.UsageText, child.UsageText, sibling.UsageText, inputs
				));
			});
		}


	}
	public sealed record RuntimeCommandSource(IRuntimeDataProvider data, IRuntimeExecutionServices execServices);
}
